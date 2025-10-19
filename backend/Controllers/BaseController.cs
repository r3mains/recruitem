using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

public abstract class BaseController : ControllerBase
{
  protected Guid? GetCurrentUserId()
  {
    var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
              User.FindFirstValue(ClaimTypes.Name) ??
              User.FindFirstValue("sub");
    return Guid.TryParse(sub, out var userId) ? userId : null;
  }

  protected string? GetCurrentUserRole()
  {
    return User.FindFirstValue("role");
  }

  protected IActionResult NotFoundIfNull<T>(T? entity, string? message = null)
  {
    return entity == null ? NotFound(message) : Ok(entity);
  }

  protected IActionResult UnauthorizedIfNull<T>(T? entity)
  {
    return entity == null ? Unauthorized() : Ok(entity);
  }
}
