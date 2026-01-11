namespace backend.DTOs.Reports;

public class ExperienceWiseCandidateDto
{
  public string ExperienceRange { get; set; } = string.Empty;
  public int MinYears { get; set; }
  public int MaxYears { get; set; }
  public int CandidateCount { get; set; }
  public List<CandidateSummary> Candidates { get; set; } = new();
}

public class CandidateSummary
{
  public Guid Id { get; set; }
  public string? FullName { get; set; }
  public string? Email { get; set; }
  public int TotalExperience { get; set; }
  public List<string> Skills { get; set; } = new();
}
