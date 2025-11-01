using Backend.Dtos.JobApplications;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ScreeningController(IScreeningService screeningService) : ControllerBase
{
  [HttpGet("applications")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<List<JobApplicationDto>>> GetApplicationsForScreening(
      [FromQuery] Guid? jobId = null,
      [FromQuery] Guid? statusId = null)
  {
    var applications = await screeningService.GetApplicationsForScreeningAsync(jobId, statusId);
    return Ok(applications);
  }

  [HttpPost("screen/{applicationId}")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<JobApplicationDto>> ScreenApplication(
      Guid applicationId, 
      ScreeningDto screeningDto)
  {
    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                           throw new UnauthorizedAccessException());

    var result = await screeningService.ScreenApplicationAsync(applicationId, screeningDto, userId);
    
    if (result == null)
      return NotFound();

    return Ok(result);
  }

  [HttpPost("bulk-screen")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<List<JobApplicationDto>>> BulkScreenApplications(
      BulkScreeningDto bulkScreeningDto)
  {
    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                           throw new UnauthorizedAccessException());

    var results = await screeningService.BulkScreenApplicationsAsync(bulkScreeningDto, userId);
    return Ok(results);
  }

  [HttpPost("shortlist")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<List<JobApplicationDto>>> ShortlistApplications(
      ShortlistingDto shortlistingDto)
  {
    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                           throw new UnauthorizedAccessException());

    var results = await screeningService.ShortlistApplicationsAsync(shortlistingDto, userId);
    return Ok(results);
  }

  [HttpGet("shortlisted")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<List<JobApplicationDto>>> GetShortlistedApplications(
      [FromQuery] Guid? jobId = null)
  {
    var applications = await screeningService.GetShortlistedApplicationsAsync(jobId);
    return Ok(applications);
  }

  [HttpGet("calculate-score/{applicationId}")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<double>> CalculateApplicationScore(Guid applicationId)
  {
    var score = await screeningService.CalculateApplicationScoreAsync(applicationId);
    return Ok(score);
  }

  [HttpGet("by-score-range")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<List<JobApplicationDto>>> GetApplicationsByScoreRange(
      [FromQuery] Guid jobId,
      [FromQuery] double minScore,
      [FromQuery] double maxScore)
  {
    var applications = await screeningService.GetApplicationsByScoreRangeAsync(jobId, minScore, maxScore);
    return Ok(applications);
  }
}
