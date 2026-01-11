using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using backend.Repositories.IRepositories;
using backend.DTOs.Position;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("positions")]
[Authorize]
public class PositionController(IPositionRepository positionRepository, IEmployeeRepository employeeRepository, ApplicationDbContext context, IMapper mapper) : ControllerBase
{
  private readonly IPositionRepository _positionRepository = positionRepository;
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;
  private readonly ApplicationDbContext _context = context;
  private readonly IMapper _mapper = mapper;

  [HttpGet]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetPositions([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
      [FromQuery] string? search = null, [FromQuery] Guid? statusId = null)
  {
    try
    {
      var positions = await _positionRepository.GetPositionsAsync(page, pageSize, search, statusId);
      var totalCount = await _positionRepository.GetPositionCountAsync(statusId);

      return Ok(new
      {
        Positions = positions,
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
      return BadRequest($"Unable to retrieve positions. Please try again. Details: {ex.Message}");
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetPosition(Guid id)
  {
    try
    {
      var position = await _positionRepository.GetPositionDetailsByIdAsync(id);
      if (position == null)
      {
        return NotFound("The position you're looking for could not be found");
      }

      return Ok(position);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to retrieve position details. Please try again. Details: {ex.Message}");
    }
  }

  [HttpPost]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> CreatePosition([FromBody] CreatePositionDto createPositionDto)
  {
    try
    {
      var position = _mapper.Map<Position>(createPositionDto);

      var openStatus = await GetPositionStatusByName(Consts.PositionStatus.Open);
      position.StatusId = openStatus.Id;

      var createdPosition = await _positionRepository.CreatePositionAsync(position);

      await AddPositionSkillsAsync(createdPosition.Id, createPositionDto.Skills.Select(s => s.SkillId).ToList());

      var positionDetails = await _positionRepository.GetPositionDetailsByIdAsync(createdPosition.Id);
      return CreatedAtAction(nameof(GetPosition), new { id = createdPosition.Id }, positionDetails);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to create position. Please try again or contact support. Details: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> UpdatePosition(Guid id, [FromBody] UpdatePositionDto updatePositionDto)
  {
    try
    {
      var existingPosition = await _positionRepository.GetPositionByIdAsync(id);
      if (existingPosition == null)
      {
        return NotFound("The position you're trying to update could not be found");
      }

      if (!string.IsNullOrEmpty(updatePositionDto.Title))
        existingPosition.Title = updatePositionDto.Title;

      if (updatePositionDto.NumberOfInterviews.HasValue)
        existingPosition.NumberOfInterviews = updatePositionDto.NumberOfInterviews.Value;

      if (updatePositionDto.StatusId.HasValue)
        existingPosition.StatusId = updatePositionDto.StatusId.Value;

      if (updatePositionDto.ReviewerId.HasValue)
        existingPosition.ReviewerId = updatePositionDto.ReviewerId;

      if (!string.IsNullOrEmpty(updatePositionDto.ClosedReason))
        existingPosition.ClosedReason = updatePositionDto.ClosedReason;

      if (updatePositionDto.SelectedCandidateId.HasValue)
        existingPosition.SelectedCandidateId = updatePositionDto.SelectedCandidateId;

      existingPosition.UpdatedAt = DateTime.UtcNow;

      var updatedPosition = await _positionRepository.UpdatePositionAsync(existingPosition);

      if (updatePositionDto.Skills != null)
      {
        await UpdatePositionSkillsAsync(id, updatePositionDto.Skills.Select(s => s.SkillId).ToList());
      }

      var positionDetails = await _positionRepository.GetPositionDetailsByIdAsync(id);
      return Ok(positionDetails);
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to update position. Please try again or contact support. Details: {ex.Message}");
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> DeletePosition(Guid id)
  {
    try
    {
      if (!await _positionRepository.ExistsAsync(id))
      {
        return NotFound("The position you're trying to delete could not be found");
      }

      await _positionRepository.DeletePositionAsync(id);
      return NoContent();
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to delete position. Please try again or contact support. Details: {ex.Message}");
    }
  }

  [HttpPost("{id}/close")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> ClosePosition(Guid id, [FromBody] ClosePositionRequest request)
  {
    try
    {
      var position = await _positionRepository.GetPositionByIdAsync(id);
      if (position == null)
      {
        return NotFound("The position you're trying to close could not be found");
      }

      // Validate that either a reason or selected candidate is provided
      if (string.IsNullOrWhiteSpace(request.Reason) && !request.SelectedCandidateId.HasValue)
      {
        return BadRequest("Either a closure reason or selected candidate must be provided when closing a position");
      }

      // If a candidate is selected, verify they exist
      if (request.SelectedCandidateId.HasValue)
      {
        var candidateExists = await _context.Candidates.AnyAsync(c => c.Id == request.SelectedCandidateId.Value && !c.IsDeleted);
        if (!candidateExists)
        {
          return BadRequest("The selected candidate could not be found");
        }
      }

      var closedStatus = await GetPositionStatusByName(Consts.PositionStatus.Closed);
      position.StatusId = closedStatus.Id;
      position.ClosedReason = request.Reason;
      position.SelectedCandidateId = request.SelectedCandidateId;
      position.UpdatedAt = DateTime.UtcNow;

      await _positionRepository.UpdatePositionAsync(position);
      
      var positionDetails = await _positionRepository.GetPositionDetailsByIdAsync(id);
      return Ok(new { message = "Position closed successfully", position = positionDetails });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to close position. Please try again. Details: {ex.Message}");
    }
  }

  [HttpPost("{id}/hold")]
  [Authorize(Policy = "ChangeStatus")]
  public async Task<IActionResult> HoldPosition(Guid id, [FromBody] HoldPositionRequest? request = null)
  {
    try
    {
      var position = await _positionRepository.GetPositionByIdAsync(id);
      if (position == null)
      {
        return NotFound("The position you're trying to put on hold could not be found");
      }

      var holdStatus = await GetPositionStatusByName(Consts.PositionStatus.OnHold);
      position.StatusId = holdStatus.Id;
      position.ClosedReason = request?.Reason;
      position.UpdatedAt = DateTime.UtcNow;

      await _positionRepository.UpdatePositionAsync(position);
      return Ok(new { message = "Position put on hold" });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to put position on hold. Please try again. Details: {ex.Message}");
    }
  }

  [HttpPost("{id}/reopen")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> ReopenPosition(Guid id)
  {
    try
    {
      var position = await _positionRepository.GetPositionByIdAsync(id);
      if (position == null)
      {
        return NotFound("The position you're trying to reopen could not be found");
      }

      var openStatus = await GetPositionStatusByName(Consts.PositionStatus.Open);
      position.StatusId = openStatus.Id;
      position.ClosedReason = null;
      position.SelectedCandidateId = null;
      position.UpdatedAt = DateTime.UtcNow;

      await _positionRepository.UpdatePositionAsync(position);
      return Ok(new { message = "Position reopened successfully" });
    }
    catch (Exception ex)
    {
      return BadRequest($"Unable to reopen position. Please try again. Details: {ex.Message}");
    }
  }

  private async Task<Models.PositionStatus> GetPositionStatusByName(string statusName)
  {
    var status = await _context.PositionStatuses.FirstOrDefaultAsync(ps => ps.Status == statusName);
    return status ?? throw new InvalidOperationException($"Position status '{statusName}' not found");
  }

  private async Task AddPositionSkillsAsync(Guid positionId, List<Guid> skillIds)
  {
    if (skillIds == null || !skillIds.Any()) return;

    var positionSkills = skillIds.Select(skillId => new PositionSkill
    {
      Id = Guid.NewGuid(),
      PositionId = positionId,
      SkillId = skillId
    });

    await _context.PositionSkills.AddRangeAsync(positionSkills);
    await _context.SaveChangesAsync();
  }

  private async Task UpdatePositionSkillsAsync(Guid positionId, List<Guid> skillIds)
  {
    var existingSkills = await _context.PositionSkills.Where(ps => ps.PositionId == positionId).ToListAsync();
    _context.PositionSkills.RemoveRange(existingSkills);

    await AddPositionSkillsAsync(positionId, skillIds);
  }
}

public class ClosePositionRequest
{
  public string? Reason { get; set; }
  public Guid? SelectedCandidateId { get; set; }
}

public class HoldPositionRequest
{
  public string? Reason { get; set; }
}
