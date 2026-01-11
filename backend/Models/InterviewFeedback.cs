using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewFeedback
  {
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public Guid ForSkill { get; set; }
    public Skill Skill { get; set; } = null!;

    public Guid FeedbackBy { get; set; }
    public Employee FeedbackByEmployee { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Feedback { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
