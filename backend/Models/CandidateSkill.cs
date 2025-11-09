using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class CandidateSkill
  {
    public Guid Id { get; set; }

    [Required]
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    [Required]
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public int? YearOfExperience { get; set; }
  }
}
