using Microsoft.AspNetCore.Mvc;
using Backend.Dtos.Positions;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
  private readonly AppDbContext _context;

  public PositionsController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<PositionDto>>> GetAll()
  {
    var positions = await _context.Positions
        .Select(p => new PositionDto
        {
          Id = p.Id,
          Title = p.Title,
          StatusId = p.StatusId,
          ClosedReason = p.ClosedReason,
          NumberOfInterviews = p.NumberOfInterviews,
          ReviewerId = p.ReviewerId
        })
        .ToListAsync();

    return Ok(positions);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PositionDto>> GetById(Guid id)
  {
    var position = await _context.Positions
        .Where(p => p.Id == id)
        .Select(p => new PositionDto
        {
          Id = p.Id,
          Title = p.Title,
          StatusId = p.StatusId,
          ClosedReason = p.ClosedReason,
          NumberOfInterviews = p.NumberOfInterviews,
          ReviewerId = p.ReviewerId
        })
        .FirstOrDefaultAsync();

    if (position == null)
      return NotFound();

    return Ok(position);
  }

  [HttpPost]
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

    var result = new PositionDto
    {
      Id = position.Id,
      Title = position.Title,
      StatusId = position.StatusId,
      ClosedReason = position.ClosedReason,
      NumberOfInterviews = position.NumberOfInterviews,
      ReviewerId = position.ReviewerId
    };

    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
  }
}
