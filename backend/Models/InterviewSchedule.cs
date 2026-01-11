using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewSchedule
  {
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public string? Location { get; set; }

    public string? MeetingLink { get; set; }

    public Guid StatusId { get; set; }
    public ScheduleStatus Status { get; set; } = null!;

    public Guid CreatedBy { get; set; }
    public Employee CreatedByEmployee { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
