using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Screening;
using backend.Repositories.IRepositories;
using backend.Validators.ScreeningValidators;
using backend.Services.IServices;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class ScreeningController(
  IScreeningRepository screeningRepository,
  ICandidateRepository candidateRepository,
  IJobApplicationRepository jobApplicationRepository,
  IPositionRepository positionRepository,
  IEmployeeRepository employeeRepository,
  IMapper mapper,
  ScreenResumeValidator screenResumeValidator,
  AddCommentValidator addCommentValidator,
  UpdateCandidateSkillsValidator updateCandidateSkillsValidator,
  AssignReviewerValidator assignReviewerValidator,
  ShortlistCandidateValidator shortlistCandidateValidator,
  IEmailService emailService) : ControllerBase
{
  private readonly IScreeningRepository _screeningRepository = screeningRepository;
  private readonly ICandidateRepository _candidateRepository = candidateRepository;
  private readonly IJobApplicationRepository _jobApplicationRepository = jobApplicationRepository;
  private readonly IPositionRepository _positionRepository = positionRepository;
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;
  private readonly IMapper _mapper = mapper;
  private readonly ScreenResumeValidator _screenResumeValidator = screenResumeValidator;
  private readonly AddCommentValidator _addCommentValidator = addCommentValidator;
  private readonly UpdateCandidateSkillsValidator _updateCandidateSkillsValidator = updateCandidateSkillsValidator;
  private readonly AssignReviewerValidator _assignReviewerValidator = assignReviewerValidator;
  private readonly ShortlistCandidateValidator _shortlistCandidateValidator = shortlistCandidateValidator;
  private readonly IEmailService _emailService = emailService;


  [HttpGet("applications")]
  [Authorize(Policy = "ScreenResumes")]
  public async Task<ActionResult<IEnumerable<ScreeningResponseDto>>> GetApplicationsForScreening(
    [FromQuery] Guid? positionId = null,
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Invalid pagination parameters. Page must be 1 or greater, and page size must be between 1 and 100");
    }

    var applications = await _screeningRepository.GetApplicationsForScreeningAsync(positionId, search, page, pageSize);
    var screeningDtos = _mapper.Map<IEnumerable<ScreeningResponseDto>>(applications);

    return Ok(new
    {
      Applications = screeningDtos,
      Pagination = new
      {
        CurrentPage = page,
        PageSize = pageSize
      }
    });
  }

  [HttpPost("screen")]
  [Authorize(Policy = "ScreenResumes")]
  public async Task<ActionResult<ScreeningResponseDto>> ScreenResume(ScreenResumeDto screenResumeDto)
  {
    var validationResult = await _screenResumeValidator.ValidateAsync(screenResumeDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var userId = GetCurrentUserId();
    if (!userId.HasValue)
    {
      return Unauthorized();
    }

    try
    {
      // Check for previous screenings before processing
      var currentApplication = await _jobApplicationRepository.GetByIdAsync(screenResumeDto.JobApplicationId);
      if (currentApplication != null)
      {
        var previousApplications = await _jobApplicationRepository.GetAllAsync(
          candidateId: currentApplication.CandidateId,
          pageSize: 1000);
        var previousScreenings = previousApplications
          .Where(app => app.Id != currentApplication.Id && app.Comments.Any())
          .ToList();

        if (previousScreenings.Any())
        {
          // Send email notification to reviewer/recruiter about previous screening history
          var reviewer = await _employeeRepository.GetEmployeeByUserIdAsync(userId.Value);
          if (reviewer?.User?.Email != null)
          {
            var candidate = await _candidateRepository.GetByIdAsync(currentApplication.CandidateId);
            var previousCount = previousScreenings.Count;
            _ = Task.Run(async () => await _emailService.SendEmailAsync(new backend.Models.EmailRequest
            {
              To = reviewer.User.Email,
              ToName = reviewer.FullName,
              Subject = $"Previous Screening History Alert - {candidate?.FullName}",
              Body = $"<p>Note: This candidate <strong>{candidate?.FullName}</strong> has been screened previously for {previousCount} other position(s).</p>" +
                     $"<p>Please review their previous screening history before proceeding with the current evaluation.</p>",
              IsHtml = true
            }));
          }
        }
      }

      var screenedApplication = await _screeningRepository.ScreenResumeAsync(
        screenResumeDto.JobApplicationId,
        screenResumeDto.Score,
        screenResumeDto.Comments,
        screenResumeDto.Approved,
        userId.Value);

      var responseDto = _mapper.Map<ScreeningResponseDto>(screenedApplication);
      return Ok(responseDto);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("comments")]
  [Authorize(Policy = "AddReviewComments")]
  public async Task<ActionResult<CommentResponseDto>> AddComment(AddCommentDto addCommentDto)
  {
    var validationResult = await _addCommentValidator.ValidateAsync(addCommentDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var userId = GetCurrentUserId();
    if (!userId.HasValue)
    {
      return Unauthorized();
    }

    try
    {
      var comment = await _screeningRepository.AddCommentAsync(
        addCommentDto.JobApplicationId,
        addCommentDto.Comment,
        userId.Value);

      var commentDto = _mapper.Map<CommentResponseDto>(comment);
      return Ok(commentDto);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("shortlist")]
  [Authorize(Policy = "ShortlistCandidates")]
  public async Task<ActionResult<ShortlistResponseDto>> ShortlistCandidate(ShortlistCandidateDto shortlistDto)
  {
    var validationResult = await _shortlistCandidateValidator.ValidateAsync(shortlistDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var userId = GetCurrentUserId();
    if (!userId.HasValue)
    {
      return Unauthorized();
    }

    try
    {
      var shortlistedApplication = await _screeningRepository.ShortlistCandidateAsync(
        shortlistDto.JobApplicationId,
        shortlistDto.Comments,
        userId.Value);

      var responseDto = _mapper.Map<ShortlistResponseDto>(shortlistedApplication);
      return Ok(responseDto);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("statistics")]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<ActionResult<Dictionary<string, int>>> GetScreeningStatistics(
    [FromQuery] Guid? positionId = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null)
  {
    var stats = await _screeningRepository.GetScreeningStatsAsync(positionId, fromDate, toDate);
    return Ok(stats);
  }

  private Guid? GetCurrentUserId()
  {
    var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
    return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
  }
}
