using Backend.Dtos.Users;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService service) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<UserDto>>> GetAll()
  {
    var data = await service.GetAll();
    return Ok(data);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<UserDto>> GetById(Guid id)
  {
    var r = await service.GetById(id);
    if (r == null) return NotFound();
    return Ok(r);
  }

  [HttpPost]
  public async Task<ActionResult<UserDto>> Create(UserCreateDto dto)
  {
    var r = await service.Create(dto);
    return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<UserDto>> Update(Guid id, UserUpdateDto dto)
  {
    var r = await service.Update(id, dto);
    if (r == null) return NotFound();
    return Ok(r);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var ok = await service.Delete(id);
    if (!ok) return NotFound();
    return NoContent();
  }
}
