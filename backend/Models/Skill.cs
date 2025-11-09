namespace backend.Models;

public class Skill
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string SkillName { get; set; } = string.Empty;

  // Navigation properties
  public virtual ICollection<PositionSkill> PositionSkills { get; set; } = [];
  public virtual ICollection<JobSkill> JobSkills { get; set; } = [];
  public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
}
