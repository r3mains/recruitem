using Backend.Dtos.Employees;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeesController(IEmployeeService service) : ControllerBase
{
  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var r = await service.GetById(id);
    if (r == null) return NotFound();
    return Ok(r);
  }

  [HttpPost]
  public async Task<IActionResult> Create(EmployeeCreateDto dto)
  {
    var r = await service.Create(dto);
    return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
  }
}
