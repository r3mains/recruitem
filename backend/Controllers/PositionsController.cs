using Microsoft.AspNetCore.Mvc;
using Backend.Dtos.Positions;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PositionsController : ControllerBase
{
  private readonly AppDbContext _context;

  public PositionsController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<PositionDto>>> GetAll([FromQuery] Guid? statusId)
  {
    var query = _context.Positions.AsQueryable();

    if (statusId.HasValue)
    {
      query = query.Where(p => p.StatusId == statusId);
    }

    var positions = await query
        .Include(p => p.Status)
        .Include(p => p.Reviewer)
        .Select(p => new PositionDto
        {
          Id = p.Id,
          Title = p.Title,
          StatusId = p.StatusId,
          StatusName = p.Status!.Name,
          ClosedReason = p.ClosedReason,
          NumberOfInterviews = p.NumberOfInterviews,
          ReviewerId = p.ReviewerId,
          ReviewerName = p.Reviewer != null ? p.Reviewer.FirstName + " " + p.Reviewer.LastName : null
        })
        .ToListAsync();

    return Ok(positions);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PositionDto>> GetById(Guid id)
  {
    var position = await _context.Positions
        .Include(p => p.Status)
        .Include(p => p.Reviewer)
        .Where(p => p.Id == id)
        .Select(p => new PositionDto
        {
          Id = p.Id,
          Title = p.Title,
          StatusId = p.StatusId,
          StatusName = p.Status!.Name,
          ClosedReason = p.ClosedReason,
          NumberOfInterviews = p.NumberOfInterviews,
          ReviewerId = p.ReviewerId,
          ReviewerName = p.Reviewer != null ? p.Reviewer.FirstName + " " + p.Reviewer.LastName : null
        })
        .FirstOrDefaultAsync();

    if (position == null)
      return NotFound();

    return Ok(position);
  }

  [HttpPost]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<ActionResult<PositionDto>> Create(PositionCreateDto dto)
  {
    var position = new Position
    {
      Id = Guid.NewGuid(),
      Title = dto.Title,
      StatusId = dto.StatusId,
      ClosedReason = dto.ClosedReason,
      NumberOfInterviews = dto.NumberOfInterviews,
      ReviewerId = dto.ReviewerId
    };

    _context.Positions.Add(position);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = position.Id }, await GetPositionDto(position.Id));
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<ActionResult<PositionDto>> Update(Guid id, PositionUpdateDto dto)
  {
    var position = await _context.Positions.FindAsync(id);
    if (position == null)
      return NotFound();

    if (!string.IsNullOrEmpty(dto.Title))
      position.Title = dto.Title;

    if (dto.StatusId.HasValue)
      position.StatusId = dto.StatusId.Value;

    if (dto.ClosedReason != null)
      position.ClosedReason = dto.ClosedReason;

    if (dto.NumberOfInterviews.HasValue)
      position.NumberOfInterviews = dto.NumberOfInterviews.Value;

    if (dto.ReviewerId.HasValue)
      position.ReviewerId = dto.ReviewerId.Value;

    await _context.SaveChangesAsync();

    return Ok(await GetPositionDto(id));
  }

  [HttpPut("{id}/status")]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<ActionResult<PositionDto>> UpdateStatus(Guid id, PositionStatusUpdateDto dto)
  {
    var position = await _context.Positions.FindAsync(id);
    if (position == null)
      return NotFound();

    position.StatusId = dto.StatusId;
    position.ClosedReason = dto.ClosedReason;

    await _context.SaveChangesAsync();

    return Ok(await GetPositionDto(id));
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var position = await _context.Positions.FindAsync(id);
    if (position == null)
      return NotFound();

    _context.Positions.Remove(position);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private async Task<PositionDto> GetPositionDto(Guid id)
  {
    return await _context.Positions
        .Include(p => p.Status)
        .Include(p => p.Reviewer)
        .Where(p => p.Id == id)
        .Select(p => new PositionDto
        {
          Id = p.Id,
          Title = p.Title,
          StatusId = p.StatusId,
          StatusName = p.Status!.Name,
          ClosedReason = p.ClosedReason,
          NumberOfInterviews = p.NumberOfInterviews,
          ReviewerId = p.ReviewerId,
          ReviewerName = p.Reviewer != null ? p.Reviewer.FirstName + " " + p.Reviewer.LastName : null
        })
        .FirstAsync();
  }
}
