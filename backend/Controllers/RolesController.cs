using Backend.Dtos.Roles;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/roles")]
public class RolesController(IRoleService service) : ControllerBase
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll()
  {
    var data = await service.GetAll();
    return Ok(data);
  }
}
