using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using backend.DTOs.Scoring;
using backend.Models;
using backend.Services.IServices;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScoringController(IScoringService scoringService, IMapper mapper, ILogger<ScoringController> logger) : ControllerBase
{
  private readonly IScoringService _scoringService = scoringService;
  private readonly IMapper _mapper = mapper;
  private readonly ILogger<ScoringController> _logger = logger;

  [HttpGet("configuration/{positionId}")]
  [Authorize(Policy = "RequireHROrHigher")]
  public async Task<ActionResult<ScoringConfigurationDto>> GetScoringConfiguration(Guid positionId)
  {
    var config = await _scoringService.GetScoringConfigurationAsync(positionId);
    if (config == null)
      return NotFound("No scoring configuration found for this position");

    return Ok(_mapper.Map<ScoringConfigurationDto>(config));
  }

  [HttpPost("configuration")]
  [Authorize(Policy = "RequireHROrHigher")]
  public async Task<ActionResult<ScoringConfigurationDto>> CreateOrUpdateConfiguration([FromBody] CreateScoringConfigurationDto dto)
  {
    // Validate total weight equals 100
    var totalWeight = dto.SkillMatchWeight + dto.ExperienceWeight + dto.InterviewWeight + dto.TestWeight + dto.EducationWeight;
    if (totalWeight != 100)
      return BadRequest("Total weight must equal 100%");

    var config = _mapper.Map<ScoringConfiguration>(dto);
    config.IsActive = true;

    var result = await _scoringService.CreateOrUpdateScoringConfigurationAsync(config);
    return Ok(_mapper.Map<ScoringConfigurationDto>(result));
  }

  [HttpPost("calculate/{jobApplicationId}")]
  [Authorize(Policy = "RequireHROrHigher")]
  public async Task<ActionResult<AutomatedScoreDto>> CalculateScore(Guid jobApplicationId)
  {
    try
    {
      var score = await _scoringService.CalculateScoreAsync(jobApplicationId);
      var dto = _mapper.Map<AutomatedScoreDto>(score);
      
      // Populate additional details
      if (score.JobApplication != null)
      {
        dto.CandidateName = score.JobApplication.Candidate?.FullName;
        dto.JobTitle = score.JobApplication.Job?.Title;
      }

      return Ok(dto);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
  }

  [HttpGet("application/{jobApplicationId}")]
  [Authorize(Policy = "RequireHROrHigher")]
  public async Task<ActionResult<AutomatedScoreDto>> GetScore(Guid jobApplicationId)
  {
    var score = await _scoringService.GetScoreAsync(jobApplicationId);
    if (score == null)
      return NotFound("No score found for this application");

    var dto = _mapper.Map<AutomatedScoreDto>(score);
    
    // Populate additional details
    if (score.JobApplication != null)
    {
      dto.CandidateName = score.JobApplication.Candidate?.FullName;
      dto.JobTitle = score.JobApplication.Job?.Title;
    }

    return Ok(dto);
  }

  [HttpGet("rankings/{jobId}")]
  [Authorize(Policy = "RequireHROrHigher")]
  public async Task<ActionResult<IEnumerable<AutomatedScoreDto>>> GetJobRankings(Guid jobId)
  {
    var scores = await _scoringService.GetScoresByJobAsync(jobId);
    var dtos = _mapper.Map<IEnumerable<AutomatedScoreDto>>(scores);
    
    // Populate additional details for each
    foreach (var dto in dtos)
    {
      var score = scores.FirstOrDefault(s => s.Id == dto.Id);
      if (score?.JobApplication != null)
      {
        dto.CandidateName = score.JobApplication.Candidate?.FullName;
        dto.JobTitle = score.JobApplication.Job?.Title;
      }
    }

    return Ok(dtos);
  }
}
