using backend.DTOs;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("users")]
[Authorize(Policy = "ViewUsers")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
  private readonly IUserRepository _userRepository = userRepository;

  [HttpGet]
  public async Task<IActionResult> GetUsers([FromQuery] UserQueryDto query)
  {
    var result = await _userRepository.GetUsersAsync(query);
    return Ok(result);
  }

  // GET /api/v1/users/roles
  [HttpGet("roles")]
  [AllowAnonymous]
  public IActionResult GetAllRoles()
  {
    var roles = new[]
    {
      backend.Consts.Roles.Admin,
      backend.Consts.Roles.HR,
      backend.Consts.Roles.Recruiter,
      backend.Consts.Roles.Interviewer,
      backend.Consts.Roles.Reviewer,
      backend.Consts.Roles.Candidate,
      backend.Consts.Roles.Viewer
    };
    return Ok(roles);
  }

  // GET /api/v1/users/{id}
  [HttpGet("{id}")]
  [Authorize]
  public async Task<IActionResult> GetUser(string id)
  {
    var currentUserId = User.FindFirst("sub")?.Value;
    
    // Allow users to view their own profile, or admins to view any profile
    if (id != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("HR") && !User.IsInRole("Viewer"))
    {
      return Forbid();
    }

    var user = await _userRepository.GetUserByIdAsync(id);
    if (user == null)
    {
      return NotFound(new { message = "The requested user could not be found." });
    }

    return Ok(user);
  }

  [HttpGet("email/{email}")]
  public async Task<IActionResult> GetUserByEmail(string email)
  {
    var user = await _userRepository.GetUserByEmailAsync(email);
    if (user == null)
    {
      return NotFound(new { message = "No user found with this email address." });
    }

    return Ok(user);
  }

  [HttpPost]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
  {
    var result = await _userRepository.CreateUserAsync(request);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = "Unable to create user. " + string.Join(" ", errors), errors });
    }

    return Ok(new { message = "User account created successfully." });
  }

  // PUT /api/v1/users/{id}
  [HttpPut("{id}")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto request)
  {
    var result = await _userRepository.UpdateUserAsync(id, request);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = "Unable to update user. " + string.Join(" ", errors), errors });
    }

    return Ok(new { message = "User information updated successfully." });
  }

  // DELETE /api/v1/users/{id}
  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> DeleteUser(string id)
  {
    var result = await _userRepository.DeleteUserAsync(id);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = "Unable to delete user. " + string.Join(" ", errors), errors });
    }

    return Ok(new { message = "User account has been deleted successfully." });
  }

  // POST /api/v1/users/{id}/restore
  [HttpPost("{id}/restore")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> RestoreUser(string id)
  {
    var result = await _userRepository.RestoreUserAsync(id);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = "Unable to restore user. " + string.Join(" ", errors), errors });
    }

    return Ok(new { message = "User account has been restored successfully." });
  }

  [HttpGet("{id}/roles")]
  public async Task<IActionResult> GetUserRoles(string id)
  {
    var roles = await _userRepository.GetUserRolesAsync(id);
    return Ok(new { userId = id, roles });
  }

  // POST /api/v1/users/assign-role
  [HttpPost("assign-role")]
  [Authorize(Policy = "ManageRoles")]
  public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
  {
    var result = await _userRepository.AssignRoleAsync(request);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = $"Unable to assign role '{request.Role}'. " + string.Join(" ", errors), errors });
    }

    return Ok(new { message = $"The role '{request.Role}' has been assigned to the user successfully." });
  }

  // POST /api/v1/users/remove-role
  [HttpPost("remove-role")]
  [Authorize(Policy = "ManageRoles")]
  public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleDto request)
  {
    var result = await _userRepository.RemoveRoleAsync(request);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = $"Unable to remove role '{request.Role}'. " + string.Join(" ", errors), errors });
    }

    return Ok(new { message = $"The role '{request.Role}' has been removed from the user successfully." });
  }

  [HttpPut("{id}/roles")]
  [Authorize(Policy = "ManageRoles")]
  public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] IList<string> roles)
  {
    var request = new UpdateUserRolesDto(id, roles);
    var result = await _userRepository.UpdateUserRolesAsync(request);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "User roles updated successfully." });
  }

  // POST /api/v1/users/{id}/lock
  [HttpPost("{id}/lock")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> LockUser(string id, [FromQuery] DateTimeOffset? lockoutEnd = null)
  {
    var result = await _userRepository.LockUserAsync(id, lockoutEnd);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    var message = lockoutEnd.HasValue
      ? $"User locked until {lockoutEnd:yyyy-MM-dd HH:mm}."
      : "User locked permanently.";

    return Ok(new { message });
  }

  // POST /api/v1/users/{id}/unlock
  [HttpPost("{id}/unlock")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> UnlockUser(string id)
  {
    var result = await _userRepository.UnlockUserAsync(id);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "User unlocked successfully." });
  }

  // POST /api/v1/users/{id}/confirm-email
  [HttpPost("{id}/confirm-email")]
  [Authorize(Policy = "ManageUsers")]
  public async Task<IActionResult> ConfirmUserEmail(string id)
  {
    var result = await _userRepository.ConfirmUserEmailAsync(id);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return Ok(new { message = "User email confirmed successfully." });
  }

  // GET /api/v1/users/statistics
  [HttpGet("statistics")]
  [Authorize(Policy = "ViewUsers")]
  public async Task<IActionResult> GetUserStatistics()
  {
    var totalUsers = await _userRepository.GetTotalUsersCountAsync();
    var activeUsers = await _userRepository.GetActiveUsersCountAsync();
    var usersByRole = await _userRepository.GetUsersByRoleCountAsync();

    return Ok(new
    {
      totalUsers,
      activeUsers,
      inactiveUsers = totalUsers - activeUsers,
      usersByRole
    });
  }
}
