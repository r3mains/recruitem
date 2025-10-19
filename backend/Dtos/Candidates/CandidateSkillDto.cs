namespace Backend.Dtos.Candidates;

public class CandidateSkillDto
{
  public Guid Id { get; set; }
  public Guid CandidateId { get; set; }
  public Guid SkillId { get; set; }
  public string SkillName { get; set; } = string.Empty;
  public int YearsOfExperience { get; set; }
}

public class CandidateSkillCreateDto
{
  public Guid SkillId { get; set; }
  public int YearsOfExperience { get; set; }
}

public class CandidateSkillUpdateDto
{
  public int YearsOfExperience { get; set; }
}
