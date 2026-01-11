using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Index(nameof(PositionId), nameof(SkillId), IsUnique = true)]
public class PositionSkill
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid PositionId { get; set; }
  public Guid SkillId { get; set; }


  public virtual Position Position { get; set; } = null!;
  public virtual Skill Skill { get; set; } = null!;
}
