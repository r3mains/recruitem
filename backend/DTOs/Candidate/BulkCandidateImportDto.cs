namespace backend.DTOs.Candidate;

public class BulkCandidateImportDto
{
  public string? FullName { get; set; }
  public string? Email { get; set; }
  public string? ContactNumber { get; set; }
  public string? College { get; set; }
  public int? GraduationYear { get; set; }
  public string? Skills { get; set; } // Comma-separated
  public string? Qualifications { get; set; } // Comma-separated
  public string? City { get; set; }
  public string? State { get; set; }
  public string? Country { get; set; }
  public int RowNumber { get; set; }
}

public class BulkImportResultDto
{
  public int TotalRows { get; set; }
  public int TotalRecords { get; set; }
  public int SuccessCount { get; set; }
  public int SuccessfulRecords { get; set; }
  public int FailureCount { get; set; }
  public int FailedRecords { get; set; }
  public List<string> Errors { get; set; } = new();
  public List<string> Warnings { get; set; } = new();
  public List<Guid> CreatedCandidateIds { get; set; } = new();
}
