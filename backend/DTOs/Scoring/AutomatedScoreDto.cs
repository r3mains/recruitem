namespace backend.DTOs.Scoring;

public class AutomatedScoreDto
{
  public Guid Id { get; set; }
  public Guid JobApplicationId { get; set; }
  public decimal SkillMatchScore { get; set; }
  public decimal ExperienceScore { get; set; }
  public decimal InterviewScore { get; set; }
  public decimal TestScore { get; set; }
  public decimal EducationScore { get; set; }
  public decimal TotalWeightedScore { get; set; }
  public string ScoreBreakdown { get; set; } = string.Empty;
  public DateTime CalculatedAt { get; set; }
  public string? CandidateName { get; set; }
  public string? JobTitle { get; set; }
}
