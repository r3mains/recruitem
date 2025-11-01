using Microsoft.AspNetCore.Mvc;
using Backend.Dtos.Positions;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PositionsController : BaseController
{
  private readonly IPositionService _positionService;

  public PositionsController(IPositionService positionService)
  {
    _positionService = positionService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<PositionDto>>> GetAll()
  {
    var positions = await _positionService.GetAll();
    return Ok(positions);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var position = await _positionService.GetById(id);
    return NotFoundIfNull(position);
  }

  [HttpPost]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<ActionResult<PositionDto>> Create(PositionCreateDto dto)
  {
    var position = await _positionService.Create(dto);
    return CreatedAtAction(nameof(GetById), new { id = position.Id }, position);
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<IActionResult> Update(Guid id, PositionUpdateDto dto)
  {
    var position = await _positionService.Update(id, dto);
    return NotFoundIfNull(position);
  }

  [HttpPut("{id}/status")]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<IActionResult> UpdateStatus(Guid id, PositionStatusUpdateDto dto)
  {
    var position = await _positionService.UpdateStatus(id, dto);
    return NotFoundIfNull(position);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var success = await _positionService.Delete(id);
    if (!success) return NotFound();
    return NoContent();
  }
}
