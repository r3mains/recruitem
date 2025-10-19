namespace Backend.Models;

public class JobSkill
{
  public Guid Id { get; set; }
  public Guid JobId { get; set; }
  public Guid SkillId { get; set; }
  public bool IsRequired { get; set; }
  public int? MinYearsExperience { get; set; }

  public Job? Job { get; set; }
  public Skill? Skill { get; set; }
}
