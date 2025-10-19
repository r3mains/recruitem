using Backend.Dtos.JobApplications;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class JobApplicationsController(JobApplicationService jobApplicationService, AppDbContext context) : ControllerBase
{
  private readonly AppDbContext _context = context;
  [HttpGet]
  [Authorize]
  public async Task<ActionResult<List<JobApplicationDto>>> GetJobApplications(
      [FromQuery] Guid? jobId = null,
      [FromQuery] Guid? candidateId = null,
      [FromQuery] Guid? statusId = null)
  {
    var applications = await jobApplicationService.GetAllAsync(jobId, candidateId, statusId);
    return Ok(applications);
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<JobApplicationDto>> GetJobApplication(Guid id)
  {
    var application = await jobApplicationService.GetByIdAsync(id);
    if (application == null)
      return NotFound();

    return Ok(application);
  }

  [HttpPost]
  [Authorize]
  public async Task<ActionResult<JobApplicationDto>> CreateApplication(JobApplicationCreateDto createDto)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
      return Unauthorized("User ID not found in token");

    var userGuid = Guid.Parse(userId);

    var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userGuid);

    if (candidate == null)
    {
      var user = await _context.Users.FindAsync(userGuid);
      if (user == null)
        return Unauthorized("User not found");

      candidate = new Candidate
      {
        Id = Guid.NewGuid(),
        UserId = userGuid,
        FullName = user.Email,
        ContactNumber = null,
        ResumeUrl = null
      };

      _context.Candidates.Add(candidate);
      await _context.SaveChangesAsync();
    }

    createDto.CandidateId = candidate.Id;

    if (!await jobApplicationService.CanApplyAsync(createDto.JobId, candidate.Id))
      return BadRequest("You have already applied for this job");

    var application = await jobApplicationService.CreateAsync(createDto);
    return CreatedAtAction(nameof(GetJobApplication), new { id = application.Id }, application);
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<JobApplicationDto>> UpdateApplication(Guid id, JobApplicationUpdateDto updateDto)
  {
    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());

    var application = await jobApplicationService.UpdateAsync(id, updateDto, userId);
    if (application == null)
      return NotFound();

    return Ok(application);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "RequireAdminOrRecruiter")]
  public async Task<IActionResult> DeleteApplication(Guid id)
  {
    var success = await jobApplicationService.DeleteAsync(id);
    if (!success)
      return NotFound();

    return NoContent();
  }

  [HttpGet("my-applications")]
  [Authorize]
  public async Task<ActionResult<List<JobApplicationDto>>> GetMyApplications()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
      return Unauthorized("User ID not found in token");

    var userGuid = Guid.Parse(userId);

    var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userGuid);

    if (candidate == null)
    {
      return Ok(new List<JobApplicationDto>());
    }

    var applications = await jobApplicationService.GetAllAsync(candidateId: candidate.Id);
    return Ok(applications);
  }

  [HttpPost("{id}/screen")]
  [Authorize(Policy = "RequireRecruitmentStaff")]
  public async Task<ActionResult<JobApplicationDto>> ScreenApplication(Guid id, JobApplicationUpdateDto updateDto)
  {
    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());

    var application = await jobApplicationService.UpdateAsync(id, updateDto, userId);
    if (application == null)
      return NotFound();

    return Ok(application);
  }
}
