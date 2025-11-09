using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class ApplicationStatusHistory
  {
    public Guid Id { get; set; }

    [Required]
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    [Required]
    public Guid StatusId { get; set; }
    public ApplicationStatus Status { get; set; } = null!;

    [Required]
    public DateTime ChangedAt { get; set; }

    public Guid? ChangedBy { get; set; }
    public User? ChangedByUser { get; set; }

    public string? Note { get; set; }
  }
}
