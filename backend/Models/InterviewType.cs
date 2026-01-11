using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewType
  {
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
  }
}
