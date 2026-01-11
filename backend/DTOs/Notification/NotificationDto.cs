namespace backend.DTOs.Notification;

public class NotificationDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = null!;
  public string Message { get; set; } = null!;
  public string Type { get; set; } = "Info";
  public string? RelatedEntityType { get; set; }
  public Guid? RelatedEntityId { get; set; }
  public bool IsRead { get; set; }
  public DateTime CreatedAt { get; set; }
}
