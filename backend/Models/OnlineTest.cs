using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
  public class OnlineTest
  {
    public Guid Id { get; set; }

    [Required]
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Score { get; set; }

    public string? Result { get; set; }

    public DateTime? TakenAt { get; set; }
  }
}
