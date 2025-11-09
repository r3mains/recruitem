namespace backend.Models;

public class ScheduleStatus
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Status { get; set; } = string.Empty;
}
