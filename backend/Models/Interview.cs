using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Interview
  {
    public Guid Id { get; set; }

    [Required]
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    [Required]
    public Guid InterviewTypeId { get; set; }
    public InterviewType InterviewType { get; set; } = null!;

    public Guid? StatusId { get; set; }
    public InterviewStatus? Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<Interviewer> Interviewers { get; set; } = new List<Interviewer>();
    public ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
    public ICollection<InterviewFeedback> InterviewFeedbacks { get; set; } = new List<InterviewFeedback>();
  }
}
