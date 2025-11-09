using backend.DTOs;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("auth")]
public class AuthController(IAuthRepository authRepository) : ControllerBase
{
  private readonly IAuthRepository _authRepository = authRepository;

  // POST /api/v1/auth/register
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterDto request)
  {
    var result = await _authRepository.RegisterUserAsync(request);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "User registered successfully" });
  }

  // POST /api/v1/auth/login
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto request)
  {
    var (success, token, roles) = await _authRepository.LoginAsync(request);
    if (!success)
    {
      return Unauthorized(new { message = "Invalid email or password" });
    }

    return Ok(new { accessToken = token, roles });
  }

  // GET /api/v1/auth/me
  [HttpGet("me")]
  [Authorize]
  public async Task<IActionResult> Me()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    if (userId == null)
    {
      return Unauthorized();
    }

    var userProfile = await _authRepository.GetUserProfileAsync(userId);
    if (userProfile == null)
    {
      return NotFound();
    }

    return Ok(userProfile);
  }

  // POST /api/v1/auth/forgot-password
  [HttpPost("forgot-password")]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
  {
    var (success, token, userId) = await _authRepository.GeneratePasswordResetTokenAsync(request.Email);

    return Ok(new
    {
      message = "If the email exists in our system, a password reset link has been sent to your email address.",
    });
  }

  // POST /api/v1/auth/reset-password
  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
  {
    var result = await _authRepository.ResetPasswordAsync(request);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "Password has been reset successfully." });
  }

  // POST /api/v1/auth/change-password
  [HttpPost("change-password")]
  [Authorize]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    if (userId == null)
    {
      return Unauthorized();
    }

    var result = await _authRepository.ChangePasswordAsync(userId, request);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "Password changed successfully." });
  }

  // POST /api/v1/auth/confirm-email
  [HttpPost("confirm-email")]
  public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto request)
  {
    var result = await _authRepository.ConfirmEmailAsync(request);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "Email confirmed successfully." });
  }

  // POST /api/v1/auth/resend-confirmation
  [HttpPost("resend-confirmation")]
  public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDto request)
  {
    var (success, token, userId) = await _authRepository.GenerateEmailConfirmationTokenAsync(request.Email);

    return Ok(new
    {
      message = "If the email exists and is not yet confirmed, a confirmation link has been sent.",
    });
  }
}
