using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewStatusHistory
  {
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public Guid StatusId { get; set; }
    public InterviewStatus Status { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public Guid? ChangedBy { get; set; }
    public User? ChangedByUser { get; set; }

    public string? Note { get; set; }
  }
}
