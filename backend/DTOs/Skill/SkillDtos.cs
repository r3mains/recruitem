namespace backend.DTOs.Skill
{
  public class CreateSkillDto
  {
    public string SkillName { get; set; } = string.Empty;
  }

  public class UpdateSkillDto
  {
    public string? SkillName { get; set; }
  }

  public class SkillResponseDto
  {
    public Guid Id { get; set; }
    public string SkillName { get; set; } = string.Empty;

    public int PositionCount { get; set; }
    public int JobCount { get; set; }
    public int CandidateCount { get; set; }
  }

  public class SkillListDto
  {
    public Guid Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
  }
}
