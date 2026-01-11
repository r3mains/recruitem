using backend.Data;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class NotificationRepository(ApplicationDbContext context) : INotificationRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false)
  {
    var query = _context.Notifications
      .Where(n => n.UserId == userId)
      .OrderByDescending(n => n.CreatedAt);

    if (unreadOnly)
    {
      query = (IOrderedQueryable<Notification>)query.Where(n => !n.IsRead);
    }

    return await query.Take(50).ToListAsync();
  }

  public async Task<Notification?> GetByIdAsync(Guid id)
  {
    return await _context.Notifications.FindAsync(id);
  }

  public async Task<Notification> CreateAsync(Notification notification)
  {
    _context.Notifications.Add(notification);
    await _context.SaveChangesAsync();
    return notification;
  }

  public async Task MarkAsReadAsync(Guid id)
  {
    var notification = await _context.Notifications.FindAsync(id);
    if (notification != null)
    {
      notification.IsRead = true;
      await _context.SaveChangesAsync();
    }
  }

  public async Task MarkAllAsReadAsync(Guid userId)
  {
    var notifications = await _context.Notifications
      .Where(n => n.UserId == userId && !n.IsRead)
      .ToListAsync();

    foreach (var notification in notifications)
    {
      notification.IsRead = true;
    }

    await _context.SaveChangesAsync();
  }

  public async Task<int> GetUnreadCountAsync(Guid userId)
  {
    return await _context.Notifications
      .CountAsync(n => n.UserId == userId && !n.IsRead);
  }

  public async Task DeleteAsync(Guid id)
  {
    var notification = await _context.Notifications.FindAsync(id);
    if (notification != null)
    {
      _context.Notifications.Remove(notification);
      await _context.SaveChangesAsync();
    }
  }
}
