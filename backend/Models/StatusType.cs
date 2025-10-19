namespace Backend.Models;

public class StatusType
{
  public Guid Id { get; set; }
  public string Context { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public string Name => Status;
}
