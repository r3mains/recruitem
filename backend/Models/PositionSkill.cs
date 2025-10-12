namespace Backend.Models;

public class PositionSkill
{
  public Guid Id { get; set; }
  public Guid PositionId { get; set; }
  public Guid SkillId { get; set; }

  public Position? Position { get; set; }
  public Skill? Skill { get; set; }
}
