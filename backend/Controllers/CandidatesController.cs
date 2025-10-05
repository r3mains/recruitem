using Backend.Dtos.Candidates;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/candidates")]
[Authorize]
public class CandidatesController(ICandidateService service) : ControllerBase
{
  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var r = await service.GetById(id);
    if (r == null) return NotFound();
    return Ok(r);
  }

  [HttpPost]
  public async Task<IActionResult> Create(CandidateCreateDto dto)
  {
    var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("sub");
    if (Guid.TryParse(sub, out var userId))
    {
      dto.UserId = userId;
    }
    var r = await service.Create(dto);
    return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
  }
}
