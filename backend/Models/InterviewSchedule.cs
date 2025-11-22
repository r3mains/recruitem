using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class InterviewSchedule
  {
    public Guid Id { get; set; }

    [Required]
    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    [Required]
    public DateTime ScheduledAt { get; set; }

    [MaxLength(255)]
    public string? Location { get; set; }

    [MaxLength(500)]
    public string? MeetingLink { get; set; }

    [Required]
    public Guid StatusId { get; set; }
    public ScheduleStatus Status { get; set; } = null!;

    [Required]
    public Guid CreatedBy { get; set; }
    public Employee CreatedByEmployee { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
