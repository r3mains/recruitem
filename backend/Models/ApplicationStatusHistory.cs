using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class ApplicationStatusHistory
  {
    public Guid Id { get; set; }

    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    public Guid StatusId { get; set; }
    public ApplicationStatus Status { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public Guid? ChangedBy { get; set; }
    public User? ChangedByUser { get; set; }

    public string? Note { get; set; }
  }
}
