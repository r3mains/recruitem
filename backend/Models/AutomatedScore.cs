namespace backend.Models;

public class AutomatedScore
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid JobApplicationId { get; set; }
  public decimal SkillMatchScore { get; set; }
  public decimal ExperienceScore { get; set; }
  public decimal InterviewScore { get; set; }
  public decimal TestScore { get; set; }
  public decimal EducationScore { get; set; }
  public decimal TotalWeightedScore { get; set; }
  public string? ScoreBreakdown { get; set; }
  public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
  public bool IsDeleted { get; set; } = false;

  public virtual JobApplication JobApplication { get; set; } = null!;
}
