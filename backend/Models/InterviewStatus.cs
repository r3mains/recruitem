using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewStatus
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Status { get; set; } = string.Empty;

    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
  }
}
