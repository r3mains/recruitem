namespace Backend.Models;

public class CandidateSkill
{
  public Guid Id { get; set; }
  public Guid CandidateId { get; set; }
  public Guid SkillId { get; set; }
  public int YearsOfExperience { get; set; }

  public Candidate? Candidate { get; set; }
  public Skill? Skill { get; set; }
}
