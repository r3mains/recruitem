namespace backend.Models;

public class JobSkill
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid JobId { get; set; }
  public Guid SkillId { get; set; }
  public bool Required { get; set; } = false;

  // Navigation properties
  public virtual Job Job { get; set; } = null!;
  public virtual Skill Skill { get; set; } = null!;
}
