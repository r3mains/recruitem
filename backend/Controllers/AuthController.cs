using Backend.Dtos.Auth;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : BaseController
{
  [HttpPost("login")]
  [AllowAnonymous]
  public async Task<IActionResult> Login(LoginRequestDto dto)
  {
    var result = await auth.Login(dto);
    return UnauthorizedIfNull(result);
  }

  [HttpPost("register")]
  [AllowAnonymous]
  public async Task<IActionResult> Register(RegisterRequestDto dto)
  {
    var result = await auth.Register(dto);
    return Ok(result);
  }
}
