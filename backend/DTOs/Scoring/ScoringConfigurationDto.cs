namespace backend.DTOs.Scoring;

public class ScoringConfigurationDto
{
  public Guid Id { get; set; }
  public Guid PositionId { get; set; }
  public decimal SkillMatchWeight { get; set; }
  public decimal ExperienceWeight { get; set; }
  public decimal InterviewWeight { get; set; }
  public decimal TestWeight { get; set; }
  public decimal EducationWeight { get; set; }
  public bool IsActive { get; set; }
}

public class CreateScoringConfigurationDto
{
  public Guid PositionId { get; set; }
  public decimal SkillMatchWeight { get; set; }
  public decimal ExperienceWeight { get; set; }
  public decimal InterviewWeight { get; set; }
  public decimal TestWeight { get; set; }
  public decimal EducationWeight { get; set; }
}
