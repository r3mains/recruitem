namespace backend.DTOs.JobApplication
{
  public class CreateJobApplicationDto
  {
    public Guid JobId { get; set; }
    public Guid CandidateId { get; set; }
  }

  public class UpdateJobApplicationDto
  {
    public Guid StatusId { get; set; }
    public decimal? Score { get; set; }
    public int? NumberOfInterviewRounds { get; set; }
    public string? Comment { get; set; }
  }

  public class JobApplicationResponseDto
  {
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public Guid CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public Guid StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public int? NumberOfInterviewRounds { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public List<JobApplicationDocumentDto> Documents { get; set; } = new();
    public List<JobApplicationCommentDto> Comments { get; set; } = new();
    public List<JobApplicationStatusHistoryDto> StatusHistory { get; set; } = new();
  }

  public class JobApplicationListDto
  {
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public Guid CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public int? NumberOfInterviewRounds { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
  }

  public class JobApplicationDocumentDto
  {
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime? UploadedAt { get; set; }
  }

  public class JobApplicationCommentDto
  {
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string CommentedBy { get; set; } = string.Empty;
    public DateTime CommentedAt { get; set; }
  }

  public class JobApplicationStatusHistoryDto
  {
    public Guid Id { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
  }

  public class ApplyToJobDto
  {
    public Guid JobId { get; set; }
  }

  public class BulkApplicationActionDto
  {
    public List<Guid> ApplicationIds { get; set; } = new();
    public Guid StatusId { get; set; }
    public string? Comment { get; set; }
  }
}
