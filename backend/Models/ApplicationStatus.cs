namespace backend.Models;

public class ApplicationStatus
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Status { get; set; } = string.Empty;

  // Navigation properties
  public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
  public virtual ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
}
