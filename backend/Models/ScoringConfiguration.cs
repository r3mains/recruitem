namespace backend.Models;

public class ScoringConfiguration
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid PositionId { get; set; }
  public decimal SkillMatchWeight { get; set; } = 30.0m;
  public decimal ExperienceWeight { get; set; } = 20.0m;
  public decimal InterviewWeight { get; set; } = 30.0m;
  public decimal TestWeight { get; set; } = 15.0m;
  public decimal EducationWeight { get; set; } = 5.0m;
  public bool IsActive { get; set; } = true;
  public bool IsDeleted { get; set; } = false;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public virtual Position Position { get; set; } = null!;
}
