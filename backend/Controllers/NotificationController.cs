using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Repositories.IRepositories;
using backend.DTOs.Notification;
using AutoMapper;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class NotificationController(
  INotificationRepository notificationRepository,
  IMapper mapper) : ControllerBase
{
  private readonly INotificationRepository _notificationRepository = notificationRepository;
  private readonly IMapper _mapper = mapper;

  // GET /api/v1/notification
  [HttpGet]
  public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
  {
    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, unreadOnly);
    var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

    return Ok(notificationDtos);
  }

  // GET /api/v1/notification/unread-count
  [HttpGet("unread-count")]
  public async Task<IActionResult> GetUnreadCount()
  {
    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var count = await _notificationRepository.GetUnreadCountAsync(userId);

    return Ok(new { Count = count });
  }

  // PUT /api/v1/notification/{id}/mark-read
  [HttpPut("{id}/mark-read")]
  public async Task<IActionResult> MarkAsRead(Guid id)
  {
    await _notificationRepository.MarkAsReadAsync(id);
    return NoContent();
  }

  // PUT /api/v1/notification/mark-all-read
  [HttpPut("mark-all-read")]
  public async Task<IActionResult> MarkAllAsRead()
  {
    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    await _notificationRepository.MarkAllAsReadAsync(userId);
    return NoContent();
  }

  // DELETE /api/v1/notification/{id}
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteNotification(Guid id)
  {
    await _notificationRepository.DeleteAsync(id);
    return NoContent();
  }
}
