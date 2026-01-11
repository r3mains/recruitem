namespace backend.Models;

public class JobType
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Type { get; set; } = string.Empty;


  public virtual ICollection<Job> Jobs { get; set; } = [];
}
