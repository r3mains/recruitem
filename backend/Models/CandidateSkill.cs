using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class CandidateSkill
  {
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public int? YearOfExperience { get; set; }
  }
}
