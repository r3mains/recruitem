namespace Backend.Models;

public class Position
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public int NumberOfInterviews { get; set; }
  public Guid? ReviewerId { get; set; }

  public StatusType? Status { get; set; }
  public Employee? Reviewer { get; set; }
  public ICollection<Job> Jobs { get; set; } = [];
  public ICollection<PositionSkill> PositionSkills { get; set; } = [];
}
