namespace backend.DTOs.Interview
{
  public class CreateInterviewDto
  {
    public Guid JobApplicationId { get; set; }
    public Guid InterviewTypeId { get; set; }
    public int RoundNumber { get; set; } = 1;
    public List<Guid> InterviewerIds { get; set; } = new List<Guid>();
  }

  public class UpdateInterviewDto
  {
    public Guid? InterviewTypeId { get; set; }
    public Guid? StatusId { get; set; }
    public int? RoundNumber { get; set; }
    public List<Guid>? InterviewerIds { get; set; }
  }

  public class InterviewDto
  {
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public string? CandidateName { get; set; }
    public string? JobTitle { get; set; }
    public Guid InterviewTypeId { get; set; }
    public string? InterviewType { get; set; }
    public int RoundNumber { get; set; }
    public Guid? StatusId { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<InterviewerDto> Interviewers { get; set; } = new List<InterviewerDto>();
    public List<InterviewScheduleDto> Schedules { get; set; } = new List<InterviewScheduleDto>();
    public List<InterviewFeedbackDto> Feedbacks { get; set; } = new List<InterviewFeedbackDto>();
  }

  public class InterviewerDto
  {
    public Guid Id { get; set; }
    public Guid InterviewerId { get; set; }
    public string? InterviewerName { get; set; }
    public string? InterviewerEmail { get; set; }
  }
}
