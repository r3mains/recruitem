using Backend.Dtos.Auth;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
  [HttpPost("login")]
  [AllowAnonymous]
  public async Task<IActionResult> Login(LoginRequestDto dto)
  {
    var r = await auth.Login(dto);
    if (r == null) return Unauthorized();
    return Ok(r);
  }

  [HttpPost("register")]
  [AllowAnonymous]
  public async Task<IActionResult> Register(RegisterRequestDto dto)
  {
    var r = await auth.Register(dto);
    return Ok(r);
  }
}
