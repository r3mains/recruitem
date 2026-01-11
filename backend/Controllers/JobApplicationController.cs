using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.JobApplication;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Validators.JobApplicationValidators;
using backend.Consts;
using backend.Services.IServices;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class JobApplicationController(
  IJobApplicationRepository jobApplicationRepository,
  ICandidateRepository candidateRepository,
  IJobRepository jobRepository,
  IMapper mapper,
  CreateJobApplicationValidator createJobApplicationValidator,
  UpdateJobApplicationValidator updateJobApplicationValidator,
  ApplyToJobValidator applyToJobValidator,
  BulkApplicationActionValidator bulkApplicationActionValidator,
  IEmailService emailService) : ControllerBase
{
  private readonly IJobApplicationRepository _jobApplicationRepository = jobApplicationRepository;
  private readonly ICandidateRepository _candidateRepository = candidateRepository;
  private readonly IJobRepository _jobRepository = jobRepository;
  private readonly IMapper _mapper = mapper;
  private readonly CreateJobApplicationValidator _createJobApplicationValidator = createJobApplicationValidator;
  private readonly UpdateJobApplicationValidator _updateJobApplicationValidator = updateJobApplicationValidator;
  private readonly ApplyToJobValidator _applyToJobValidator = applyToJobValidator;
  private readonly BulkApplicationActionValidator _bulkApplicationActionValidator = bulkApplicationActionValidator;
  private readonly IEmailService _emailService = emailService;

  [HttpGet]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<ActionResult<IEnumerable<JobApplicationListDto>>> GetJobApplications(
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] Guid? jobId = null,
    [FromQuery] Guid? candidateId = null,
    [FromQuery] Guid? statusId = null)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Invalid pagination parameters. Page must be 1 or greater, and page size must be between 1 and 100");
    }

    var applications = await _jobApplicationRepository.GetAllAsync(search, page, pageSize, jobId, candidateId, statusId);
    var applicationListDtos = _mapper.Map<IEnumerable<JobApplicationListDto>>(applications);

    var totalCount = await _jobApplicationRepository.GetCountAsync(search, jobId, candidateId);
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    var result = new
    {
      Applications = applicationListDtos,
      Pagination = new
      {
        CurrentPage = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = totalPages
      }
    };

    return Ok(result);
  }

  // GET /api/v1/jobapplication/{id}
  [HttpGet("{id}")]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<ActionResult<JobApplicationResponseDto>> GetJobApplication(Guid id)
  {
    var application = await _jobApplicationRepository.GetByIdAsync(id);
    if (application == null)
    {
      return NotFound($"The job application with ID {id} could not be found");
    }

    var applicationResponseDto = _mapper.Map<JobApplicationResponseDto>(application);
    return Ok(applicationResponseDto);
  }

  [HttpPost]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult<JobApplicationResponseDto>> CreateJobApplication(CreateJobApplicationDto createJobApplicationDto)
  {
    var validationResult = await _createJobApplicationValidator.ValidateAsync(createJobApplicationDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    if (await _jobApplicationRepository.HasAppliedAsync(createJobApplicationDto.CandidateId, createJobApplicationDto.JobId))
    {
      return BadRequest("This candidate has already applied to this job posting");
    }

    var job = await _jobRepository.GetJobByIdAsync(createJobApplicationDto.JobId);
    if (job == null)
    {
      return BadRequest("The specified job posting could not be found. Please verify the job ID");
    }

    var candidate = await _candidateRepository.GetByIdAsync(createJobApplicationDto.CandidateId);
    if (candidate == null)
    {
      return BadRequest("The specified candidate profile could not be found. Please verify the candidate ID");
    }

    var jobApplication = _mapper.Map<JobApplication>(createJobApplicationDto);

    var appliedStatus = await GetApplicationStatusByNameAsync(backend.Consts.ApplicationStatus.Applied);
    jobApplication.StatusId = appliedStatus?.Id ?? Guid.NewGuid();

    var userId = GetCurrentUserId();
    if (userId.HasValue)
    {
      jobApplication.CreatedBy = userId.Value;
    }

    var createdApplication = await _jobApplicationRepository.CreateAsync(jobApplication);

    await AddStatusHistoryEntry(createdApplication.Id, null, appliedStatus?.Status ?? "Applied", userId, "Application created");

    var fullApplication = await _jobApplicationRepository.GetByIdAsync(createdApplication.Id);
    var applicationResponseDto = _mapper.Map<JobApplicationResponseDto>(fullApplication);

    return CreatedAtAction(nameof(GetJobApplication), new { id = createdApplication.Id }, applicationResponseDto);
  }

  // POST /api/v1/jobapplication/apply
  [HttpPost("apply")]
  [Authorize(Roles = "Candidate")]
  public async Task<ActionResult<JobApplicationResponseDto>> ApplyToJob(ApplyToJobDto applyToJobDto)
  {
    var validationResult = await _applyToJobValidator.ValidateAsync(applyToJobDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var userId = GetCurrentUserId();
    if (!userId.HasValue)
    {
      return Unauthorized();
    }

    var candidate = await _candidateRepository.GetByUserIdAsync(userId.Value);
    if (candidate == null)
    {
      return BadRequest("Your candidate profile could not be found. Please complete your profile before applying");
    }

    if (await _jobApplicationRepository.HasAppliedAsync(candidate.Id, applyToJobDto.JobId))
    {
      return BadRequest("You have already applied to this job posting. You cannot submit duplicate applications");
    }

    var job = await _jobRepository.GetJobByIdAsync(applyToJobDto.JobId);
    if (job == null || job.IsDeleted)
    {
      return BadRequest("The job posting you're trying to apply to could not be found or is no longer active");
    }

    var jobApplication = new JobApplication
    {
      JobId = applyToJobDto.JobId,
      CandidateId = candidate.Id,
      CreatedBy = userId.Value
    };

    var appliedStatus = await GetApplicationStatusByNameAsync(backend.Consts.ApplicationStatus.Applied);
    jobApplication.StatusId = appliedStatus?.Id ?? Guid.NewGuid();

    var createdApplication = await _jobApplicationRepository.CreateAsync(jobApplication);

    await AddStatusHistoryEntry(createdApplication.Id, null, appliedStatus?.Status ?? "Applied", userId, "Candidate applied to job");

    var fullApplication = await _jobApplicationRepository.GetByIdAsync(createdApplication.Id);
    var applicationResponseDto = _mapper.Map<JobApplicationResponseDto>(fullApplication);

    return CreatedAtAction(nameof(GetJobApplication), new { id = createdApplication.Id }, applicationResponseDto);
  }

  // PUT /api/v1/jobapplication/{id}
  [HttpPut("{id}")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult<JobApplicationResponseDto>> UpdateJobApplication(Guid id, UpdateJobApplicationDto updateJobApplicationDto)
  {
    var validationResult = await _updateJobApplicationValidator.ValidateAsync(updateJobApplicationDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var existingApplication = await _jobApplicationRepository.GetByIdAsync(id);
    if (existingApplication == null)
    {
      return NotFound($"The job application with ID {id} could not be found");
    }

    var previousStatusId = existingApplication.StatusId;
    var previousStatus = existingApplication.Status?.Status;

    _mapper.Map(updateJobApplicationDto, existingApplication);

    var userId = GetCurrentUserId();
    if (userId.HasValue)
    {
      existingApplication.UpdatedBy = userId.Value;
    }

    // Update NumberOfInterviewRounds if provided
    if (updateJobApplicationDto.NumberOfInterviewRounds.HasValue)
    {
      existingApplication.NumberOfInterviewRounds = updateJobApplicationDto.NumberOfInterviewRounds.Value;
    }

    var updatedApplication = await _jobApplicationRepository.UpdateAsync(existingApplication);

    if (previousStatusId != updateJobApplicationDto.StatusId)
    {
      var newStatus = await GetApplicationStatusByIdAsync(updateJobApplicationDto.StatusId);
      await AddStatusHistoryEntry(id, previousStatus, newStatus?.Status, userId, updateJobApplicationDto.Comment);
      
      // Send email notification for status change
      _ = SendStatusChangeEmailAsync(existingApplication, newStatus?.Status ?? "Updated", updateJobApplicationDto.Comment);
    }

    var fullApplication = await _jobApplicationRepository.GetByIdAsync(updatedApplication.Id);
    var applicationResponseDto = _mapper.Map<JobApplicationResponseDto>(fullApplication);

    return Ok(applicationResponseDto);
  }

  // POST /api/v1/jobapplication/bulk-update
  [HttpPost("bulk-update")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult> BulkUpdateApplications(BulkApplicationActionDto bulkActionDto)
  {
    var validationResult = await _bulkApplicationActionValidator.ValidateAsync(bulkActionDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var applications = await _jobApplicationRepository.GetByIdsAsync(bulkActionDto.ApplicationIds);
    if (!applications.Any())
    {
      return BadRequest("No valid job applications found for the provided IDs. Please verify your selection");
    }

    var userId = GetCurrentUserId();
    var newStatus = await GetApplicationStatusByIdAsync(bulkActionDto.StatusId);

    foreach (var application in applications)
    {
      var previousStatus = application.Status?.Status;

      application.StatusId = bulkActionDto.StatusId;
      if (userId.HasValue)
      {
        application.UpdatedBy = userId.Value;
      }

      await AddStatusHistoryEntry(application.Id, previousStatus, newStatus?.Status, userId, bulkActionDto.Comment);
      
      // Send email notification for bulk status change
      _ = SendStatusChangeEmailAsync(application, newStatus?.Status ?? "Updated", bulkActionDto.Comment);
    }

    await _jobApplicationRepository.UpdateMultipleAsync(applications);

    return Ok(new { message = $"Updated {applications.Count()} applications", updatedCount = applications.Count() });
  }

  // POST /api/v1/jobapplication/bulk-delete
  [HttpPost("bulk-delete")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult> BulkDeleteApplications([FromBody] List<Guid> applicationIds)
  {
    if (applicationIds == null || !applicationIds.Any())
    {
      return BadRequest("Application IDs are required");
    }

    var applications = await _jobApplicationRepository.GetByIdsAsync(applicationIds);
    if (!applications.Any())
    {
      return BadRequest("No valid job applications found for the provided IDs. Please verify your selection");
    }

    foreach (var application in applications)
    {
      await _jobApplicationRepository.DeleteAsync(application.Id);
    }

    return Ok(new { message = $"Deleted {applications.Count()} applications", deletedCount = applications.Count() });
  }

  // POST /api/v1/jobapplication/bulk-shortlist
  [HttpPost("bulk-shortlist")]
  [Authorize(Policy = "ShortlistCandidates")]
  public async Task<ActionResult> BulkShortlistApplications([FromBody] List<Guid> applicationIds)
  {
    if (applicationIds == null || !applicationIds.Any())
    {
      return BadRequest("At least one application ID is required to perform bulk shortlist");
    }

    var applications = await _jobApplicationRepository.GetByIdsAsync(applicationIds);
    if (!applications.Any())
    {
      return BadRequest("No valid job applications found for the provided IDs. Please verify your selection");
    }

    var shortlistedStatus = await GetApplicationStatusByNameAsync("Shortlisted");
    if (shortlistedStatus == null)
    {
      return BadRequest("The shortlisted status configuration could not be found. Please contact support");
    }

    var userId = GetCurrentUserId();

    foreach (var application in applications)
    {
      var previousStatus = application.Status?.Status;
      application.StatusId = shortlistedStatus.Id;
      
      if (userId.HasValue)
      {
        application.UpdatedBy = userId.Value;
      }

      await AddStatusHistoryEntry(application.Id, previousStatus, "Shortlisted", userId, "Bulk shortlisted");
    }

    await _jobApplicationRepository.UpdateMultipleAsync(applications);

    return Ok(new { message = $"Shortlisted {applications.Count()} applications", shortlistedCount = applications.Count() });
  }

  // POST /api/v1/jobapplication/bulk-reject
  [HttpPost("bulk-reject")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult> BulkRejectApplications([FromBody] BulkApplicationActionDto bulkActionDto)
  {
    if (bulkActionDto.ApplicationIds == null || !bulkActionDto.ApplicationIds.Any())
    {
      return BadRequest("At least one application ID is required to perform bulk reject");
    }

    var applications = await _jobApplicationRepository.GetByIdsAsync(bulkActionDto.ApplicationIds);
    if (!applications.Any())
    {
      return BadRequest("No valid job applications found for the provided IDs. Please verify your selection");
    }

    var rejectedStatus = await GetApplicationStatusByNameAsync("Rejected");
    if (rejectedStatus == null)
    {
      return BadRequest("The rejected status configuration could not be found. Please contact support");
    }

    var userId = GetCurrentUserId();

    foreach (var application in applications)
    {
      var previousStatus = application.Status?.Status;
      application.StatusId = rejectedStatus.Id;
      
      if (userId.HasValue)
      {
        application.UpdatedBy = userId.Value;
      }

      await AddStatusHistoryEntry(application.Id, previousStatus, "Rejected", userId, bulkActionDto.Comment ?? "Bulk rejected");
    }

    await _jobApplicationRepository.UpdateMultipleAsync(applications);

    return Ok(new { message = $"Rejected {applications.Count()} applications", rejectedCount = applications.Count() });
  }

  // GET /api/v1/jobapplication/my-applications
  [HttpGet("my-applications")]
  [Authorize(Roles = "Candidate")]
  public async Task<ActionResult<IEnumerable<JobApplicationListDto>>> GetMyApplications(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    var userId = GetCurrentUserId();
    if (!userId.HasValue)
    {
      return Unauthorized();
    }

    var candidate = await _candidateRepository.GetByUserIdAsync(userId.Value);
    if (candidate == null)
    {
      return BadRequest("Your candidate profile could not be found. Please contact support");
    }

    return await GetJobApplications(null, page, pageSize, null, candidate.Id, null);
  }

  [HttpGet("job/{jobId}/statistics")]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<ActionResult<Dictionary<string, int>>> GetJobApplicationStatistics(Guid jobId)
  {
    var stats = await _jobApplicationRepository.GetApplicationStatsByJobAsync(jobId);
    return Ok(stats);
  }

  private Guid? GetCurrentUserId()
  {
    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                   ?? User.FindFirst("sub")?.Value 
                   ?? User.FindFirst("id")?.Value;
    return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
  }

  private async Task<Models.ApplicationStatus?> GetApplicationStatusByNameAsync(string statusName)
  {
    using var scope = HttpContext.RequestServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<backend.Data.ApplicationDbContext>();
    return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
      context.ApplicationStatuses.Where(s => s.Status == statusName));
  }

  private async Task<Models.ApplicationStatus?> GetApplicationStatusByIdAsync(Guid statusId)
  {
    using var scope = HttpContext.RequestServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<backend.Data.ApplicationDbContext>();
    return await context.ApplicationStatuses.FindAsync(statusId);
  }

  private async Task AddStatusHistoryEntry(Guid applicationId, string? fromStatus, string? toStatus, Guid? changedBy, string? note)
  {
    var statusHistory = new ApplicationStatusHistory
    {
      JobApplicationId = applicationId,
      ChangedAt = DateTime.UtcNow,
      Note = note,
      ChangedBy = changedBy
    };

    if (!string.IsNullOrEmpty(toStatus))
    {
      var status = await GetApplicationStatusByNameAsync(toStatus);
      if (status != null)
      {
        statusHistory.StatusId = status.Id;
      }
    }

    await _jobApplicationRepository.AddStatusHistoryAsync(statusHistory);
  }

  private async Task SendStatusChangeEmailAsync(JobApplication application, string newStatus, string? comments)
  {
    try
    {
      var candidate = application.Candidate ?? await _candidateRepository.GetByIdAsync(application.CandidateId);
      var job = application.Job ?? await _jobRepository.GetJobByIdAsync(application.JobId);
      
      if (candidate?.User?.Email != null && job != null)
      {
        await _emailService.SendApplicationStatusUpdateAsync(
          candidate.User.Email,
          candidate.FullName ?? "Candidate",
          job.Title,
          newStatus,
          comments
        );
      }
    }
    catch (Exception ex)
    {
      // Log error but don't fail the request
      Console.WriteLine($"Failed to send status change email: {ex.Message}");
    }
  }
}
