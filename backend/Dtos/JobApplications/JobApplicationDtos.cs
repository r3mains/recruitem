namespace Backend.Dtos.JobApplications;

public class JobApplicationDto
{
  public Guid Id { get; set; }
  public Guid JobId { get; set; }
  public string JobTitle { get; set; } = string.Empty;
  public Guid CandidateId { get; set; }
  public string CandidateName { get; set; } = string.Empty;
  public string CandidateEmail { get; set; } = string.Empty;
  public DateTime AppliedAt { get; set; }
  public Guid StatusId { get; set; }
  public string StatusName { get; set; } = string.Empty;
  public string? CoverLetter { get; set; }
  public string? Notes { get; set; }
  public double? Score { get; set; }
  public DateTime? ReviewedAt { get; set; }
  public Guid? ReviewedBy { get; set; }
  public string? ReviewerName { get; set; }
}

public class JobApplicationCreateDto
{
  public Guid JobId { get; set; }
  public Guid CandidateId { get; set; }
  public string? CoverLetter { get; set; }
}

public class JobApplicationUpdateDto
{
  public Guid StatusId { get; set; }
  public string? Notes { get; set; }
  public double? Score { get; set; }
}

public class ScreeningDto
{
  public Guid StatusId { get; set; }
  public string? Notes { get; set; }
  public double? Score { get; set; }
}

public class ShortlistingDto
{
  public List<Guid> ApplicationIds { get; set; } = [];
  public Guid StatusId { get; set; }
  public string? Notes { get; set; }
}

public class BulkScreeningDto
{
  public List<Guid> ApplicationIds { get; set; } = [];
  public Guid StatusId { get; set; }
  public string? Notes { get; set; }
  public double? MinScore { get; set; }
  public double? MaxScore { get; set; }
}
