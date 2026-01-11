namespace backend.Models;

public class Notification
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string Title { get; set; } = null!;
  public string Message { get; set; } = null!;
  public string Type { get; set; } = "Info"; // Info, Success, Warning, Error
  public string? RelatedEntityType { get; set; } // JobApplication, Interview, etc.
  public Guid? RelatedEntityId { get; set; }
  public bool IsRead { get; set; } = false;
  public DateTime CreatedAt { get; set; }

  public User User { get; set; } = null!;
}

public class StageNotificationTemplate
{
  public string Stage { get; set; } = null!;
  public string EmailSubject { get; set; } = null!;
  public string EmailBody { get; set; } = null!;
  public string NotificationTitle { get; set; } = null!;
  public string NotificationMessage { get; set; } = null!;
  public List<string> NotifyRoles { get; set; } = new();
}
