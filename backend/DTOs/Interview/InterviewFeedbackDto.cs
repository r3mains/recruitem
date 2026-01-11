namespace backend.DTOs.Interview
{
  public class CreateInterviewFeedbackDto
  {
    public Guid InterviewId { get; set; }
    public Guid ForSkill { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
  }

  public class UpdateInterviewFeedbackDto
  {
    public int? Rating { get; set; }
    public string? Feedback { get; set; }
  }

  public class InterviewFeedbackDto
  {
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public Guid ForSkill { get; set; }
    public string? SkillName { get; set; }
    public Guid? FeedbackBy { get; set; }
    public string? FeedbackByName { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
