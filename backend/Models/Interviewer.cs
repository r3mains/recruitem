using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Interviewer
  {
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public Guid InterviewerId { get; set; }
    public Employee InterviewerEmployee { get; set; } = null!;
  }
}
