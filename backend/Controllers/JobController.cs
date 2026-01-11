using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using backend.Repositories.IRepositories;
using backend.DTOs.Job;
using backend.Models;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("jobs")]
[Authorize]
public class JobController(IJobRepository jobRepository, IEmployeeRepository employeeRepository, IMapper mapper) : ControllerBase
{
  private readonly IJobRepository _jobRepository = jobRepository;
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;
  private readonly IMapper _mapper = mapper;

  [HttpPost]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> CreateJob([FromBody] CreateJobDto createJobDto)
  {
    try
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
      {
        return Unauthorized("Invalid user");
      }

      var employee = await _employeeRepository.GetEmployeeByUserIdAsync(userId);
      if (employee == null)
      {
        return BadRequest("Your employee profile could not be found. Please contact support");
      }

      var job = _mapper.Map<Job>(createJobDto);
      job.RecruiterId = employee.Id;

      var openStatus = await GetJobStatusByName(Consts.JobStatus.Open);
      job.StatusId = openStatus.Id;

      var createdJob = await _jobRepository.CreateJobAsync(job);

      await AddJobSkillsAsync(createdJob.Id, createJobDto.RequiredSkillIds, true);
      await AddJobSkillsAsync(createdJob.Id, createJobDto.PreferredSkillIds, false);
      await AddJobQualificationsAsync(createdJob.Id, createJobDto.Qualifications);

      var jobDetails = await _jobRepository.GetJobDetailsByIdAsync(createdJob.Id);
      return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, jobDetails);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to create job posting. Please try again or contact support if the issue persists. Details: {ex.Message}");
    }
  }

  [HttpGet]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
      [FromQuery] string? search = null, [FromQuery] Guid? statusId = null, [FromQuery] Guid? jobTypeId = null)
  {
    try
    {
      var jobs = await _jobRepository.GetJobsAsync(page, pageSize, search, statusId, jobTypeId);
      var totalCount = await _jobRepository.GetJobCountAsync(statusId);

      return Ok(new
      {
        Jobs = jobs,
        Pagination = new
        {
          Page = page,
          PageSize = pageSize,
          TotalCount = totalCount,
          TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        }
      });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve job listings. Please try again. Details: {ex.Message}");
    }
  }

  [HttpGet("public/{id}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetPublicJob(Guid id)
  {
    try
    {
      var job = await _jobRepository.GetJobDetailsByIdAsync(id);
      if (job == null)
      {
        return NotFound("The job posting you're looking for could not be found");
      }

      // Only return open jobs for public view
      var openStatus = await GetJobStatusByName(Consts.JobStatus.Open);
      if (job.Status?.Id != openStatus.Id)
      {
        return NotFound("This job posting is not available");
      }

      return Ok(job);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve job details. Please try again. Details: {ex.Message}");
    }
  }

  [HttpGet("public")]
  [AllowAnonymous]
  public async Task<IActionResult> GetPublicJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
      [FromQuery] string? search = null, [FromQuery] Guid? jobTypeId = null)
  {
    try
    {
      // Get only open job postings for public view
      var openStatus = await GetJobStatusByName(Consts.JobStatus.Open);
      var jobs = await _jobRepository.GetJobsAsync(page, pageSize, search, openStatus.Id, jobTypeId);
      var totalCount = await _jobRepository.GetJobCountAsync(openStatus.Id);

      return Ok(new
      {
        Jobs = jobs,
        Pagination = new
        {
          Page = page,
          PageSize = pageSize,
          TotalCount = totalCount,
          TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        }
      });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve job listings. Please try again. Details: {ex.Message}");
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetJob(Guid id)
  {
    try
    {
      var job = await _jobRepository.GetJobDetailsByIdAsync(id);
      if (job == null)
      {
        return NotFound("The job posting you're looking for could not be found");
      }

      return Ok(job);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve job details. Please try again. Details: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobDto updateJobDto)
  {
    try
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
      {
        return Unauthorized("Unable to verify your identity. Please log in again");
      }

      var existingJob = await _jobRepository.GetJobByIdAsync(id);
      if (existingJob == null)
      {
        return NotFound("The job posting you're trying to update could not be found");
      }

      var isAdmin = User.IsInRole(Consts.Roles.Admin);
      if (!isAdmin && !await _jobRepository.CanUpdateJobAsync(id, userId))
      {
        return Forbid("You don't have permission to update this job");
      }

      if (!string.IsNullOrEmpty(updateJobDto.Title))
        existingJob.Title = updateJobDto.Title;

      if (!string.IsNullOrEmpty(updateJobDto.Description))
        existingJob.Description = updateJobDto.Description;

      if (updateJobDto.JobTypeId.HasValue)
        existingJob.JobTypeId = updateJobDto.JobTypeId.Value;

      if (updateJobDto.AddressId.HasValue)
        existingJob.AddressId = updateJobDto.AddressId.Value;

      if (updateJobDto.SalaryMin.HasValue)
        existingJob.SalaryMin = updateJobDto.SalaryMin;

      if (updateJobDto.SalaryMax.HasValue)
        existingJob.SalaryMax = updateJobDto.SalaryMax;

      if (updateJobDto.StatusId.HasValue)
        existingJob.StatusId = updateJobDto.StatusId.Value;

      if (!string.IsNullOrEmpty(updateJobDto.ClosedReason))
        existingJob.ClosedReason = updateJobDto.ClosedReason;

      existingJob.UpdatedAt = DateTime.UtcNow;

      var updatedJob = await _jobRepository.UpdateJobAsync(existingJob);

      if (updateJobDto.RequiredSkillIds != null || updateJobDto.PreferredSkillIds != null)
      {
        await UpdateJobSkillsAsync(id, updateJobDto.RequiredSkillIds, updateJobDto.PreferredSkillIds);
      }

      if (updateJobDto.Qualifications != null)
      {
        await UpdateJobQualificationsAsync(id, updateJobDto.Qualifications);
      }

      var jobDetails = await _jobRepository.GetJobDetailsByIdAsync(id);
      return Ok(jobDetails);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to update job posting. Please try again or contact support if the issue persists. Details: {ex.Message}");
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> DeleteJob(Guid id)
  {
    try
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
      {
        return Unauthorized("Unable to verify your identity. Please log in again");
      }

      if (!await _jobRepository.ExistsAsync(id))
      {
        return NotFound("The job posting you're trying to delete could not be found");
      }

      var isAdmin = User.IsInRole(Consts.Roles.Admin);
      if (!isAdmin && !await _jobRepository.CanDeleteJobAsync(id, userId))
      {
        return Forbid("You don't have permission to delete this job");
      }

      await _jobRepository.DeleteJobAsync(id);
      return NoContent();
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to delete job posting. Please try again or contact support. Details: {ex.Message}");
    }
  }

  [HttpPost("{id}/close")]
  [Authorize(Policy = "CloseJobs")]
  public async Task<IActionResult> CloseJob(Guid id, [FromBody] string? reason = null)
  {
    try
    {
      var job = await _jobRepository.GetJobByIdAsync(id);
      if (job == null)
      {
        return NotFound("The job posting you're trying to close could not be found");
      }

      var closedStatus = await GetJobStatusByName(Consts.JobStatus.Closed);
      job.StatusId = closedStatus.Id;
      job.ClosedReason = reason;
      job.UpdatedAt = DateTime.UtcNow;

      await _jobRepository.UpdateJobAsync(job);
      return Ok(new { message = "Job closed successfully" });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to close job posting. Please try again. Details: {ex.Message}");
    }
  }

  [HttpPost("{id}/hold")]
  [Authorize(Policy = "ChangeStatus")]
  public async Task<IActionResult> HoldJob(Guid id)
  {
    try
    {
      var job = await _jobRepository.GetJobByIdAsync(id);
      if (job == null)
      {
        return NotFound("The job posting you're trying to put on hold could not be found");
      }

      var holdStatus = await GetJobStatusByName(Consts.JobStatus.OnHold);
      job.StatusId = holdStatus.Id;
      job.UpdatedAt = DateTime.UtcNow;

      await _jobRepository.UpdateJobAsync(job);
      return Ok(new { message = "Job put on hold" });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to put job posting on hold. Please try again. Details: {ex.Message}");
    }
  }

  [HttpGet("recruiter/{recruiterId}")]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetJobsByRecruiter(Guid recruiterId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
  {
    try
    {
      var jobs = await _jobRepository.GetJobsByRecruiterAsync(recruiterId, page, pageSize);
      var totalCount = await _jobRepository.GetJobCountAsync(recruiterId: recruiterId);

      return Ok(new
      {
        Jobs = jobs,
        Pagination = new
        {
          Page = page,
          PageSize = pageSize,
          TotalCount = totalCount,
          TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        }
      });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve recruiter's job postings. Please try again. Details: {ex.Message}");
    }
  }

  [HttpGet("position/{positionId}")]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetJobsByPosition(Guid positionId)
  {
    try
    {
      var jobs = await _jobRepository.GetJobsByPositionAsync(positionId);
      return Ok(jobs);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve jobs for this position. Please try again. Details: {ex.Message}");
    }
  }

  private async Task<Models.JobStatus> GetJobStatusByName(string statusName)
  {
    var context = HttpContext.RequestServices.GetRequiredService<Data.ApplicationDbContext>();
    var status = await context.JobStatuses.FirstOrDefaultAsync(js => js.Status == statusName);
    return status ?? throw new InvalidOperationException($"Job status '{statusName}' not found");
  }

  private async Task AddJobSkillsAsync(Guid jobId, List<Guid> skillIds, bool required)
  {
    if (skillIds == null || !skillIds.Any()) return;

    var context = HttpContext.RequestServices.GetRequiredService<Data.ApplicationDbContext>();
    var jobSkills = skillIds.Select(skillId => new JobSkill
    {
      Id = Guid.NewGuid(),
      JobId = jobId,
      SkillId = skillId,
      Required = required
    });

    await context.JobSkills.AddRangeAsync(jobSkills);
    await context.SaveChangesAsync();
  }

  private async Task AddJobQualificationsAsync(Guid jobId, List<JobQualificationDto> qualifications)
  {
    if (qualifications == null || !qualifications.Any()) return;

    var context = HttpContext.RequestServices.GetRequiredService<Data.ApplicationDbContext>();
    var jobQualifications = qualifications.Select(q => new JobQualification
    {
      Id = Guid.NewGuid(),
      JobId = jobId,
      QualificationId = q.QualificationId,
      MinRequired = q.MinRequired
    });

    await context.JobQualifications.AddRangeAsync(jobQualifications);
    await context.SaveChangesAsync();
  }

  private async Task UpdateJobSkillsAsync(Guid jobId, List<Guid>? requiredSkillIds, List<Guid>? preferredSkillIds)
  {
    var context = HttpContext.RequestServices.GetRequiredService<Data.ApplicationDbContext>();

    var existingSkills = await context.JobSkills.Where(js => js.JobId == jobId).ToListAsync();
    context.JobSkills.RemoveRange(existingSkills);

    if (requiredSkillIds != null && requiredSkillIds.Any())
    {
      await AddJobSkillsAsync(jobId, requiredSkillIds, true);
    }

    if (preferredSkillIds != null && preferredSkillIds.Any())
    {
      await AddJobSkillsAsync(jobId, preferredSkillIds, false);
    }
  }

  private async Task UpdateJobQualificationsAsync(Guid jobId, List<JobQualificationDto> qualifications)
  {
    var context = HttpContext.RequestServices.GetRequiredService<Data.ApplicationDbContext>();

    var existingQualifications = await context.JobQualifications.Where(jq => jq.JobId == jobId).ToListAsync();
    context.JobQualifications.RemoveRange(existingQualifications);

    await AddJobQualificationsAsync(jobId, qualifications);
  }
}
