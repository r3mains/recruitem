using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using backend.Repositories.IRepositories;
using backend.DTOs.Interview;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("interviews")]
[Authorize]
public class InterviewController : ControllerBase
{
  private readonly IInterviewRepository _interviewRepository;

  public InterviewController(IInterviewRepository interviewRepository)
  {
    _interviewRepository = interviewRepository;
  }

  [HttpGet]
  [Authorize(Policy = "ViewInterviews")]
  public async Task<IActionResult> GetAllInterviews()
  {
    try
    {
      var interviews = await _interviewRepository.GetAllInterviewsAsync();
      return Ok(interviews);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving interviews.", error = ex.Message });
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewInterviews")]
  public async Task<IActionResult> GetInterview(Guid id)
  {
    try
    {
      var interview = await _interviewRepository.GetInterviewByIdAsync(id);
      if (interview == null)
        return NotFound(new { message = "Interview not found." });

      return Ok(interview);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the interview.", error = ex.Message });
    }
  }

  [HttpGet("job-application/{jobApplicationId}")]
  [Authorize(Policy = "ViewInterviews")]
  public async Task<IActionResult> GetInterviewsByJobApplication(Guid jobApplicationId)
  {
    try
    {
      var interviews = await _interviewRepository.GetInterviewsByJobApplicationIdAsync(jobApplicationId);
      return Ok(interviews);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving interviews.", error = ex.Message });
    }
  }

  [HttpPost]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> CreateInterview([FromBody] CreateInterviewDto dto)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return Unauthorized();

      var interview = await _interviewRepository.CreateInterviewAsync(dto, Guid.Parse(userId));
      return CreatedAtAction(nameof(GetInterview), new { id = interview.Id }, interview);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the interview.", error = ex.Message });
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> UpdateInterview(Guid id, [FromBody] UpdateInterviewDto dto)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var interview = await _interviewRepository.UpdateInterviewAsync(id, dto);
      if (interview == null)
        return NotFound(new { message = "Interview not found." });

      return Ok(interview);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the interview.", error = ex.Message });
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> DeleteInterview(Guid id)
  {
    try
    {
      var success = await _interviewRepository.DeleteInterviewAsync(id);
      if (!success)
        return NotFound(new { message = "Interview not found." });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the interview.", error = ex.Message });
    }
  }

  [HttpGet("{interviewId}/schedules")]
  [Authorize(Policy = "ViewInterviews")]
  public async Task<IActionResult> GetInterviewSchedules(Guid interviewId)
  {
    try
    {
      var schedules = await _interviewRepository.GetSchedulesByInterviewIdAsync(interviewId);
      return Ok(schedules);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving interview schedules.", error = ex.Message });
    }
  }

  [HttpGet("schedules/{scheduleId}")]
  [Authorize(Policy = "ViewInterviews")]
  public async Task<IActionResult> GetSchedule(Guid scheduleId)
  {
    try
    {
      var schedule = await _interviewRepository.GetScheduleByIdAsync(scheduleId);
      if (schedule == null)
        return NotFound(new { message = "Interview schedule not found." });

      return Ok(schedule);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the interview schedule.", error = ex.Message });
    }
  }

  [HttpPost("schedules")]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> CreateSchedule([FromBody] CreateInterviewScheduleDto dto)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return Unauthorized();

      var schedule = await _interviewRepository.CreateScheduleAsync(dto, Guid.Parse(userId));
      return CreatedAtAction(nameof(GetSchedule), new { scheduleId = schedule.Id }, schedule);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the interview schedule.", error = ex.Message });
    }
  }

  [HttpPut("schedules/{scheduleId}")]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> UpdateSchedule(Guid scheduleId, [FromBody] UpdateInterviewScheduleDto dto)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var schedule = await _interviewRepository.UpdateScheduleAsync(scheduleId, dto);
      if (schedule == null)
        return NotFound(new { message = "Interview schedule not found." });

      return Ok(schedule);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the interview schedule.", error = ex.Message });
    }
  }

  [HttpDelete("schedules/{scheduleId}")]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> DeleteSchedule(Guid scheduleId)
  {
    try
    {
      var success = await _interviewRepository.DeleteScheduleAsync(scheduleId);
      if (!success)
        return NotFound(new { message = "Interview schedule not found." });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the interview schedule.", error = ex.Message });
    }
  }

  [HttpGet("{interviewId}/feedback")]
  [Authorize(Policy = "ViewInterviewFeedback")]
  public async Task<IActionResult> GetInterviewFeedback(Guid interviewId)
  {
    try
    {
      var feedback = await _interviewRepository.GetFeedbackByInterviewIdAsync(interviewId);
      return Ok(feedback);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving interview feedback.", error = ex.Message });
    }
  }

  [HttpGet("feedback/{feedbackId}")]
  [Authorize(Policy = "ViewInterviewFeedback")]
  public async Task<IActionResult> GetFeedback(Guid feedbackId)
  {
    try
    {
      var feedback = await _interviewRepository.GetFeedbackByIdAsync(feedbackId);
      if (feedback == null)
        return NotFound(new { message = "Interview feedback not found." });

      return Ok(feedback);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the interview feedback.", error = ex.Message });
    }
  }

  [HttpPost("feedback")]
  [Authorize(Policy = "AddInterviewFeedback")]
  public async Task<IActionResult> CreateFeedback([FromBody] CreateInterviewFeedbackDto dto)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return Unauthorized();

      var feedback = await _interviewRepository.CreateFeedbackAsync(dto, Guid.Parse(userId));
      return CreatedAtAction(nameof(GetFeedback), new { feedbackId = feedback.Id }, feedback);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the interview feedback.", error = ex.Message });
    }
  }

  [HttpPut("feedback/{feedbackId}")]
  [Authorize(Policy = "AddInterviewFeedback")]
  public async Task<IActionResult> UpdateFeedback(Guid feedbackId, [FromBody] UpdateInterviewFeedbackDto dto)
  {
    try
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var feedback = await _interviewRepository.UpdateFeedbackAsync(feedbackId, dto);
      if (feedback == null)
        return NotFound(new { message = "Interview feedback not found." });

      return Ok(feedback);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the interview feedback.", error = ex.Message });
    }
  }

  [HttpDelete("feedback/{feedbackId}")]
  [Authorize(Policy = "AddInterviewFeedback")]
  public async Task<IActionResult> DeleteFeedback(Guid feedbackId)
  {
    try
    {
      var success = await _interviewRepository.DeleteFeedbackAsync(feedbackId);
      if (!success)
        return NotFound(new { message = "Interview feedback not found." });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the interview feedback.", error = ex.Message });
    }
  }

  [HttpGet("types")]
  public async Task<IActionResult> GetInterviewTypes()
  {
    try
    {
      var types = await _interviewRepository.GetInterviewTypesAsync();
      return Ok(types);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving interview types.", error = ex.Message });
    }
  }

  [HttpPost("schedule")]
  [Authorize(Policy = "ScheduleInterviews")]
  public async Task<IActionResult> ScheduleInterview([FromBody] CreateInterviewDto dto)
  {
    return await CreateInterview(dto);
  }

  [HttpPost("{interviewId}/conduct")]
  [Authorize(Policy = "ConductInterviews")]
  public async Task<IActionResult> ConductInterview(Guid interviewId)
  {
    try
    {
      var updateDto = new UpdateInterviewDto
      {
        StatusId = GetStatusIdByName("In Progress")
      };

      var interview = await _interviewRepository.UpdateInterviewAsync(interviewId, updateDto);
      if (interview == null)
        return NotFound(new { message = "Interview not found." });

      return Ok(new { message = "Interview status updated to 'In Progress'", interview });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating interview status.", error = ex.Message });
    }
  }

