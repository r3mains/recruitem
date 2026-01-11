namespace backend.Models;

public class EmailTemplate
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string Category { get; set; } = string.Empty;
  public string? AvailableVariables { get; set; }
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
