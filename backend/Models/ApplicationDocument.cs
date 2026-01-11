using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class ApplicationDocument
  {
    public Guid Id { get; set; }

    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public string? Note { get; set; }
  }
}
