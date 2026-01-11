using Microsoft.AspNetCore.Identity;

namespace backend.Models;

public class User : IdentityUser<Guid>
{
  public DateTimeOffset? CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }
  public bool IsDeleted { get; set; }

  public Employee? Employee { get; set; }
  public Candidate? Candidate { get; set; }
  public ICollection<Document> UploadedDocuments { get; set; } = new List<Document>();
  public ICollection<JobApplication> CreatedJobApplications { get; set; } = new List<JobApplication>();
  public ICollection<JobApplication> UpdatedJobApplications { get; set; } = new List<JobApplication>();
  public ICollection<ApplicationStatusHistory> StatusChanges { get; set; } = new List<ApplicationStatusHistory>();
}
