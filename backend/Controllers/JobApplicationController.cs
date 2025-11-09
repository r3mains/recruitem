using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.JobApplication;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Validators.JobApplicationValidators;
using backend.Consts;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "UserPolicy")]
public class JobApplicationController : ControllerBase
{
  private readonly IJobApplicationRepository _jobApplicationRepository;
  private readonly ICandidateRepository _candidateRepository;
  private readonly IJobRepository _jobRepository;
  private readonly IMapper _mapper;
  private readonly CreateJobApplicationValidator _createJobApplicationValidator;
  private readonly UpdateJobApplicationValidator _updateJobApplicationValidator;
  private readonly ApplyToJobValidator _applyToJobValidator;
  private readonly BulkApplicationActionValidator _bulkApplicationActionValidator;

  public JobApplicationController(
    IJobApplicationRepository jobApplicationRepository,
    ICandidateRepository candidateRepository,
    IJobRepository jobRepository,
    IMapper mapper,
    CreateJobApplicationValidator createJobApplicationValidator,
    UpdateJobApplicationValidator updateJobApplicationValidator,
    ApplyToJobValidator applyToJobValidator,
    BulkApplicationActionValidator bulkApplicationActionValidator)
  {
    _jobApplicationRepository = jobApplicationRepository;
    _candidateRepository = candidateRepository;
    _jobRepository = jobRepository;
    _mapper = mapper;
    _createJobApplicationValidator = createJobApplicationValidator;
    _updateJobApplicationValidator = updateJobApplicationValidator;
    _applyToJobValidator = applyToJobValidator;
    _bulkApplicationActionValidator = bulkApplicationActionValidator;
  }

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
      return BadRequest("Page must be >= 1 and PageSize must be between 1 and 100");
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
      return NotFound($"Job application with ID {id} not found");
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
      return BadRequest("Candidate has already applied to this job");
    }

    var job = await _jobRepository.GetJobByIdAsync(createJobApplicationDto.JobId);
    if (job == null)
    {
      return BadRequest("Invalid job ID");
    }

    var candidate = await _candidateRepository.GetByIdAsync(createJobApplicationDto.CandidateId);
    if (candidate == null)
    {
      return BadRequest("Invalid candidate ID");
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
      return BadRequest("Candidate profile not found");
    }

    if (await _jobApplicationRepository.HasAppliedAsync(candidate.Id, applyToJobDto.JobId))
    {
      return BadRequest("You have already applied to this job");
    }

    var job = await _jobRepository.GetJobByIdAsync(applyToJobDto.JobId);
    if (job == null || job.IsDeleted)
    {
      return BadRequest("Job not found or no longer active");
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
      return NotFound($"Job application with ID {id} not found");
    }

    var previousStatusId = existingApplication.StatusId;
    var previousStatus = existingApplication.Status?.Status;

    _mapper.Map(updateJobApplicationDto, existingApplication);

    var userId = GetCurrentUserId();
    if (userId.HasValue)
    {
      existingApplication.UpdatedBy = userId.Value;
    }

    var updatedApplication = await _jobApplicationRepository.UpdateAsync(existingApplication);

    if (previousStatusId != updateJobApplicationDto.StatusId)
    {
      var newStatus = await GetApplicationStatusByIdAsync(updateJobApplicationDto.StatusId);
      await AddStatusHistoryEntry(id, previousStatus, newStatus?.Status, userId, updateJobApplicationDto.Comment);
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
      return BadRequest("No valid applications found for the provided IDs");
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
    }

    await _jobApplicationRepository.UpdateMultipleAsync(applications);

    return Ok(new { message = $"Updated {applications.Count()} applications", updatedCount = applications.Count() });
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
      return BadRequest("Candidate profile not found");
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
    var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
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
}
