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
    var role = request.Role ?? backend.Consts.Roles.Candidate;
    var result = await _authRepository.RegisterUserAsync(request, role);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = string.Join(", ", errors), errors });
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
      return Unauthorized(new { message = "Your session has expired. Please log in again." });
    }

    var userProfile = await _authRepository.GetUserProfileAsync(userId);
    if (userProfile == null)
    {
      return NotFound(new { message = "User profile not found. Please contact support." });
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
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = string.Join(" ", errors), errors });
    }

    return Ok(new { message = "Your password has been reset successfully. You can now log in with your new password." });
  }

  // POST /api/v1/auth/change-password
  [HttpPost("change-password")]
  [Authorize]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    if (userId == null)
    {
      return Unauthorized(new { message = "Your session has expired. Please log in again." });
    }

    var result = await _authRepository.ChangePasswordAsync(userId, request);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = string.Join(" ", errors), errors });
    }

    return Ok(new { message = "Your password has been changed successfully." });
  }

  // POST /api/v1/auth/confirm-email
  [HttpPost("confirm-email")]
  public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto request)
  {
    var result = await _authRepository.ConfirmEmailAsync(request);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = "Unable to confirm email. The link may be invalid or expired.", errors });
    }

    return Ok(new { message = "Your email has been confirmed successfully! You can now log in." });
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
