namespace backend.Models;

public class JobStatus
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Status { get; set; } = string.Empty;

  // Navigation properties
  public virtual ICollection<Job> Jobs { get; set; } = [];
}
