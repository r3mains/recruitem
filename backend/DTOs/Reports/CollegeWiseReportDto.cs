namespace backend.DTOs.Reports;

public class CollegeWiseReportDto
{
  public string College { get; set; } = string.Empty;
  public int TotalCandidates { get; set; }
  public int TotalApplications { get; set; }
  public int ShortlistedCount { get; set; }
  public int InterviewedCount { get; set; }
  public int HiredCount { get; set; }
  public decimal SuccessRate { get; set; }
  public double? AverageTimeToHireDays { get; set; }
  public List<CollegeGraduationYearStats>? YearWiseStats { get; set; }
}

public class CollegeGraduationYearStats
{
  public int Year { get; set; }
  public int CandidateCount { get; set; }
  public int HiredCount { get; set; }
}
