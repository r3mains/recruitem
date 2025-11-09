namespace backend.Models;

public class InterviewStatus
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Status { get; set; } = string.Empty;
}
