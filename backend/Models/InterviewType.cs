using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewType
  {
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
  }
}
