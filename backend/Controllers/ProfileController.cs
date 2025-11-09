using backend.DTOs.Profile;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("profiles")]
[Authorize]
public class ProfileController(IProfileRepository profileRepository) : ControllerBase
{
  private readonly IProfileRepository _profileRepository = profileRepository;


  [HttpGet("employee")]
  [Authorize(Policy = "ViewUsers")]
  public async Task<IActionResult> GetMyEmployeeProfile()
  {
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    var profile = await _profileRepository.GetEmployeeProfileAsync(userId);
    if (profile == null)
    {
      return NotFound(new { message = "Employee profile not found." });
    }

    return Ok(profile);
  }

  [HttpGet("employee/{userId}")]
  [Authorize(Policy = "ViewUsers")]
  public async Task<IActionResult> GetEmployeeProfile(string userId)
  {
    var profile = await _profileRepository.GetEmployeeProfileAsync(userId);
    if (profile == null)
    {
      return NotFound(new { message = "Employee profile not found." });
    }

    return Ok(profile);
  }

  [HttpPost("employee")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> CreateEmployeeProfile([FromBody] CreateEmployeeDto request)
  {
    try
    {
      var profile = await _profileRepository.CreateEmployeeProfileAsync(request.UserId.ToString(), request);
      return Ok(new { message = "Employee profile created successfully.", profile });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
      return Conflict(new { message = ex.Message });
    }
  }

  [HttpPut("employee")]
  public async Task<IActionResult> UpdateMyEmployeeProfile([FromBody] UpdateEmployeeDto request)
  {
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    try
    {
      var profile = await _profileRepository.UpdateEmployeeProfileAsync(userId, request);
      return Ok(new { message = "Employee profile updated successfully.", profile });
    }
    catch (ArgumentException ex)
    {
      return NotFound(new { message = ex.Message });
    }
  }

  [HttpPut("employee/{userId}")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> UpdateEmployeeProfile(string userId, [FromBody] UpdateEmployeeDto request)
  {
    try
    {
      var profile = await _profileRepository.UpdateEmployeeProfileAsync(userId, request);
      return Ok(new { message = "Employee profile updated successfully.", profile });
    }
    catch (ArgumentException ex)
    {
      return NotFound(new { message = ex.Message });
    }
  }

  [HttpDelete("employee/{userId}")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> DeleteEmployeeProfile(string userId)
  {
    await _profileRepository.DeleteEmployeeProfileAsync(userId);
    return Ok(new { message = "Employee profile deleted successfully." });
  }

  [HttpGet("candidate")]
  [Authorize(Policy = "CandidatePortal")]
  public async Task<IActionResult> GetMyCandidateProfile()
  {
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    var profile = await _profileRepository.GetCandidateProfileAsync(userId);
    if (profile == null)
    {
      return NotFound(new { message = "Candidate profile not found." });
    }

    return Ok(profile);
  }

  [HttpGet("candidate/{userId}")]
  [Authorize(Policy = "ViewUsers")]
  public async Task<IActionResult> GetCandidateProfile(string userId)
  {
    var profile = await _profileRepository.GetCandidateProfileAsync(userId);
    if (profile == null)
    {
      return NotFound(new { message = "Candidate profile not found." });
    }

    return Ok(profile);
  }

  [HttpPost("candidate")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> CreateCandidateProfile([FromBody] CreateCandidateDto request)
  {
    try
    {
      var profile = await _profileRepository.CreateCandidateProfileAsync(request.UserId.ToString(), request);
      return Ok(new { message = "Candidate profile created successfully.", profile });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
      return Conflict(new { message = ex.Message });
    }
  }

  [HttpPut("candidate")]
  [Authorize(Policy = "CandidatePortal")]
  public async Task<IActionResult> UpdateMyCandidateProfile([FromBody] UpdateCandidateDto request)
  {
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    try
    {
      var profile = await _profileRepository.UpdateCandidateProfileAsync(userId, request);
      return Ok(new { message = "Candidate profile updated successfully.", profile });
    }
    catch (ArgumentException ex)
    {
      return NotFound(new { message = ex.Message });
    }
  }

  [HttpPut("candidate/{userId}")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> UpdateCandidateProfile(string userId, [FromBody] UpdateCandidateDto request)
  {
    try
    {
      var profile = await _profileRepository.UpdateCandidateProfileAsync(userId, request);
      return Ok(new { message = "Candidate profile updated successfully.", profile });
    }
    catch (ArgumentException ex)
    {
      return NotFound(new { message = ex.Message });
    }
  }

  [HttpDelete("candidate/{userId}")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> DeleteCandidateProfile(string userId)
  {
    await _profileRepository.DeleteCandidateProfileAsync(userId);
    return Ok(new { message = "Candidate profile deleted successfully." });
  }

  private string? GetCurrentUserId()
  {
    return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
  }
}
