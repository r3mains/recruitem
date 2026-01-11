using backend.Models;

namespace backend.Repositories.IRepositories;

public interface INotificationRepository
{
  Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false);
  Task<Notification?> GetByIdAsync(Guid id);
  Task<Notification> CreateAsync(Notification notification);
  Task MarkAsReadAsync(Guid id);
  Task MarkAllAsReadAsync(Guid userId);
  Task<int> GetUnreadCountAsync(Guid userId);
  Task DeleteAsync(Guid id);
}
