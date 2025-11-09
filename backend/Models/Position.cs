namespace backend.Models;

public class Position
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Title { get; set; } = string.Empty;
  public Guid StatusId { get; set; }
  public string? ClosedReason { get; set; }
  public int NumberOfInterviews { get; set; }
  public Guid? ReviewerId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public bool IsDeleted { get; set; } = false;

  // Navigation properties
  public virtual PositionStatus Status { get; set; } = null!;
  public virtual Employee? Reviewer { get; set; }
  public virtual ICollection<PositionSkill> PositionSkills { get; set; } = [];
  public virtual ICollection<Job> Jobs { get; set; } = [];
}
