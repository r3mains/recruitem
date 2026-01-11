using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FluentValidation;
using backend.Repositories.IRepositories;
using backend.DTOs.Interview;
using backend.Services.IServices;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("interviews")]
[Authorize]
public class InterviewController(
  IInterviewRepository interviewRepository,
  IValidator<CreateInterviewDto> createInterviewValidator,
  IValidator<CreateInterviewScheduleDto> createScheduleValidator,
  IValidator<CreateInterviewFeedbackDto> createFeedbackValidator,
  IEmailService emailService,
  IJobApplicationRepository jobApplicationRepository,
  ICandidateRepository candidateRepository,
  IEmployeeRepository employeeRepository) : ControllerBase
{
  private readonly IInterviewRepository _interviewRepository = interviewRepository;
  private readonly IValidator<CreateInterviewDto> _createInterviewValidator = createInterviewValidator;
  private readonly IValidator<CreateInterviewScheduleDto> _createScheduleValidator = createScheduleValidator;
  private readonly IValidator<CreateInterviewFeedbackDto> _createFeedbackValidator = createFeedbackValidator;
  private readonly IEmailService _emailService = emailService;
  private readonly IJobApplicationRepository _jobApplicationRepository = jobApplicationRepository;
  private readonly ICandidateRepository _candidateRepository = candidateRepository;
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;

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
        return NotFound(new { message = "The interview schedule could not be found" });

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
      var validationResult = await _createInterviewValidator.ValidateAsync(dto);
      if (!validationResult.IsValid)
      {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return BadRequest(new { message = "Unable to schedule interview. " + string.Join(" ", errors), errors });
      }

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return Unauthorized(new { message = "Your session has expired. Please log in again." });

      // Check for previous interviews before scheduling
      var currentApplication = await _jobApplicationRepository.GetByIdAsync(dto.JobApplicationId);
      if (currentApplication != null)
      {
        var candidate = await _candidateRepository.GetByIdAsync(currentApplication.CandidateId);
        var previousApplications = await _jobApplicationRepository.GetAllAsync(
          candidateId: currentApplication.CandidateId,
          pageSize: 1000);
        var previousInterviews = previousApplications
          .Where(app => app.Id != currentApplication.Id)
          .SelectMany(app => app.Interviews)
          .ToList();

        if (previousInterviews.Any())
        {
          // Send email notification to scheduler about previous interview history
          var schedulerEmail = User.FindFirst(ClaimTypes.Email)?.Value;
          var schedulerName = User.FindFirst(ClaimTypes.Name)?.Value;
          
          if (!string.IsNullOrEmpty(schedulerEmail))
          {
            var previousCount = previousInterviews.Count;
            _ = Task.Run(async () => await _emailService.SendEmailAsync(new backend.Models.EmailRequest
            {
              To = schedulerEmail,
              ToName = schedulerName,
              Subject = $"Previous Interview History Alert - {candidate?.FullName}",
              Body = $"<p>Note: This candidate <strong>{candidate?.FullName}</strong> has been interviewed previously for {previousCount} other position(s).</p>" +
                     $"<p>Please review their previous interview history and feedback before proceeding with the current interview.</p>",
              IsHtml = true
            }));
          }
        }
      }

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

      var interview = await _interviewRepository.UpdateInterviewAsync(id, dto);
      if (interview == null)
        return NotFound(new { message = "The interview schedule could not be found" });

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
        return NotFound(new { message = "The interview schedule could not be found" });

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
        return NotFound(new { message = "The interview schedule could not be found" });

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
      var validationResult = await _createScheduleValidator.ValidateAsync(dto);
      if (!validationResult.IsValid)
      {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return BadRequest(new { message = "Unable to create interview schedule. " + string.Join(" ", errors), errors });
      }

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return Unauthorized(new { message = "Your session has expired. Please log in again." });

      var schedule = await _interviewRepository.CreateScheduleAsync(dto, Guid.Parse(userId));
      
      // Send email notification for interview schedule
      _ = SendInterviewScheduleEmailAsync(dto.InterviewId, schedule.ScheduledAt);
      
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

      var schedule = await _interviewRepository.UpdateScheduleAsync(scheduleId, dto);
      if (schedule == null)
        return NotFound(new { message = "The interview schedule could not be found" });

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
        return NotFound(new { message = "The interview schedule could not be found" });

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
        return NotFound(new { message = "The interview feedback could not be found" });

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
      var validationResult = await _createFeedbackValidator.ValidateAsync(dto);
      if (!validationResult.IsValid)
      {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return BadRequest(new { message = "Unable to submit interview feedback. " + string.Join(" ", errors), errors });
      }

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return Unauthorized(new { message = "Your session has expired. Please log in again." });
      // Resolve current user to an employee record (FeedbackBy must be Employee.Id)
      var employee = await _employeeRepository.GetEmployeeByUserIdAsync(Guid.Parse(userId));
      if (employee == null)
        return BadRequest(new { message = "Unable to submit interview feedback. Your user is not linked to an employee record." });

      var feedback = await _interviewRepository.CreateFeedbackAsync(dto, employee.Id);
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

      var feedback = await _interviewRepository.UpdateFeedbackAsync(feedbackId, dto);
      if (feedback == null)
        return NotFound(new { message = "The interview feedback could not be found" });

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
        return NotFound(new { message = "The interview feedback could not be found" });

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
        StatusId = new Guid("50000000-0000-0000-0000-000000000002")
      };

      var interview = await _interviewRepository.UpdateInterviewAsync(interviewId, updateDto);
      if (interview == null)
        return NotFound(new { message = "The interview schedule could not be found" });

      return Ok(new { message = "Interview status updated to 'In Progress'", interview });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating interview status.", error = ex.Message });
    }
  }

  private async Task SendInterviewScheduleEmailAsync(Guid interviewId, DateTime scheduledAt)
  {
    try
    {
      var interview = await _interviewRepository.GetInterviewByIdAsync(interviewId);
      if (interview == null) return;

      var application = await _jobApplicationRepository.GetByIdAsync(interview.JobApplicationId);
      if (application == null) return;

      var candidate = await _candidateRepository.GetByIdAsync(application.CandidateId);
      if (candidate?.User?.Email == null) return;

      await _emailService.SendInterviewScheduledEmailAsync(
        candidate.User.Email,
        candidate.FullName ?? "Candidate",
        scheduledAt,
        interview.JobTitle ?? "Position"
      );
    }
    catch (Exception ex)
    {
      // Log error but don't fail the request
      Console.WriteLine($"Failed to send interview schedule email: {ex.Message}");
    }
  }
}
