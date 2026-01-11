namespace backend.DTOs.Interview
{
  public class CreateInterviewScheduleDto
  {
    public Guid InterviewId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
  }

  public class UpdateInterviewScheduleDto
  {
    public DateTime? ScheduledAt { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public Guid? StatusId { get; set; }
  }

  public class InterviewScheduleDto
  {
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public Guid StatusId { get; set; }
    public string? Status { get; set; }
    public Guid CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
