using backend.Repositories.IRepositories;
using backend.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("export")]
[Authorize(Policy = "GenerateReports")]
public class ExportController : ControllerBase
{
  private readonly IJobRepository _jobRepository;
  private readonly ICandidateRepository _candidateRepository;
  private readonly IJobApplicationRepository _jobApplicationRepository;
  private readonly IInterviewRepository _interviewRepository;
  private readonly IExportService _exportService;

  public ExportController(
    IJobRepository jobRepository,
    ICandidateRepository candidateRepository,
    IJobApplicationRepository jobApplicationRepository,
    IInterviewRepository interviewRepository,
    IExportService exportService)
  {
    _jobRepository = jobRepository;
    _candidateRepository = candidateRepository;
    _jobApplicationRepository = jobApplicationRepository;
    _interviewRepository = interviewRepository;
    _exportService = exportService;
  }

  [HttpGet("jobs")]
  public async Task<IActionResult> ExportJobs([FromQuery] string? search, [FromQuery] Guid? statusId, [FromQuery] Guid? jobTypeId)
  {
    try
    {
      var jobs = await _jobRepository.GetJobsAsync(1, 10000, search, statusId, jobTypeId);
      var csvData = await _exportService.ExportToCsvAsync(jobs);

      return File(csvData, "text/csv", $"jobs-export-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while exporting jobs.", error = ex.Message });
    }
  }

  [HttpGet("candidates")]
  public async Task<IActionResult> ExportCandidates([FromQuery] string? search)
  {
    try
    {
      var candidates = await _candidateRepository.GetAllAsync(search, 1, 10000);
      
      var exportData = candidates.Select(c => new
      {
        c.Id,
        c.FullName,
        Email = c.User.Email,
        ContactNumber = c.ContactNumber,
        Address = c.Address != null && c.Address.City != null 
          ? $"{c.Address.City.CityName}, {c.Address.City.State!.StateName}"
          : "",
        Skills = string.Join(", ", c.CandidateSkills.Select(cs => cs.Skill.SkillName)),
        Qualifications = string.Join(", ", c.CandidateQualifications.Select(cq => cq.Qualification.QualificationName)),
        TotalApplications = c.JobApplications.Count,
        CreatedAt = c.CreatedAt
      });

      var csvData = await _exportService.ExportToCsvAsync(exportData);

      return File(csvData, "text/csv", $"candidates-export-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while exporting candidates.", error = ex.Message });
    }
  }

  [HttpGet("applications")]
  public async Task<IActionResult> ExportApplications(
    [FromQuery] string? search,
    [FromQuery] Guid? jobId,
    [FromQuery] Guid? candidateId,
    [FromQuery] Guid? statusId)
  {
    try
    {
      var applications = await _jobApplicationRepository.GetAllAsync(search, 1, 10000, jobId, candidateId, statusId);
      
      var exportData = applications.Select(a => new
      {
        a.Id,
        JobTitle = a.Job.Title,
        CandidateName = a.Candidate.FullName,
        CandidateEmail = a.Candidate.User.Email,
        Status = a.Status.Status,
        AppliedAt = a.AppliedAt,
        LastUpdated = a.LastUpdated,
        CreatedBy = a.CreatedByUser != null ? a.CreatedByUser.UserName : "",
        UpdatedBy = a.UpdatedByUser != null ? a.UpdatedByUser.UserName : ""
      });

      var csvData = await _exportService.ExportToCsvAsync(exportData);

      return File(csvData, "text/csv", $"applications-export-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while exporting applications.", error = ex.Message });
    }
  }

  [HttpGet("interviews")]
  public async Task<IActionResult> ExportInterviews()
  {
    try
    {
      var interviews = await _interviewRepository.GetAllInterviewsAsync();
      var csvData = await _exportService.ExportToCsvAsync(interviews);

      return File(csvData, "text/csv", $"interviews-export-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while exporting interviews.", error = ex.Message });
    }
  }

  [HttpGet("skills")]
  public async Task<IActionResult> ExportSkills()
  {
    try
    {
      var skills = await _jobRepository.GetJobByIdAsync(Guid.Empty); // This is just to get context
      // We need a proper skill repository call
      return StatusCode(501, new { message = "Export skills endpoint not yet implemented" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while exporting skills.", error = ex.Message });
    }
  }
}
