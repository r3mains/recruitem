namespace Backend.Models;

public class Skill
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;

  public ICollection<JobSkill> JobSkills { get; set; } = [];
  public ICollection<PositionSkill> PositionSkills { get; set; } = [];
  public ICollection<CandidateSkill> CandidateSkills { get; set; } = [];
}
