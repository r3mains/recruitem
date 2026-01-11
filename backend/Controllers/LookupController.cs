using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("lookups")]
[Authorize]
public class LookupController(ApplicationDbContext context) : ControllerBase
{
  private readonly ApplicationDbContext _context = context;

  [HttpGet("statuses")]
  public async Task<IActionResult> GetAllStatuses()
  {
    try
    {
      var statuses = new List<object>();

      // Application Statuses
      var applicationStatuses = await _context.ApplicationStatuses
        .Select(s => new { s.Id, s.Status, Context = "application" })
        .ToListAsync();
      statuses.AddRange(applicationStatuses);

      // Position Statuses
      var positionStatuses = await _context.PositionStatuses
        .Select(s => new { s.Id, s.Status, Context = "position" })
        .ToListAsync();
      statuses.AddRange(positionStatuses);

      // Job Statuses
      var jobStatuses = await _context.JobStatuses
        .Select(s => new { s.Id, s.Status, Context = "job" })
        .ToListAsync();
      statuses.AddRange(jobStatuses);

      // Interview Statuses
      var interviewStatuses = await _context.InterviewStatuses
        .Select(s => new { s.Id, s.Status, Context = "interview" })
        .ToListAsync();
      statuses.AddRange(interviewStatuses);

      // Schedule Statuses
      var scheduleStatuses = await _context.ScheduleStatuses
        .Select(s => new { s.Id, s.Status, Context = "schedule" })
        .ToListAsync();
      statuses.AddRange(scheduleStatuses);

      // Event Candidate Statuses
      var eventStatuses = await _context.EventCandidateStatuses
        .Select(s => new { s.Id, s.Status, Context = "event" })
        .ToListAsync();
      statuses.AddRange(eventStatuses);

      // Verification Statuses
      var verificationStatuses = await _context.VerificationStatuses
        .Select(s => new { s.Id, s.Status, Context = "verification" })
        .ToListAsync();
      statuses.AddRange(verificationStatuses);

      return Ok(statuses);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve statuses. Details: {ex.Message}");
    }
  }
}
