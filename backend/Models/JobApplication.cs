namespace Backend.Models;

public class JobApplication
{
  public Guid Id { get; set; }
  public Guid JobId { get; set; }
  public Guid CandidateId { get; set; }
  public Guid StatusId { get; set; }
  public string? CoverLetterUrl { get; set; }
  public string? ResumeUrl { get; set; }
  public DateTime AppliedAt { get; set; }
  public DateTime? LastUpdated { get; set; }

  public string? Notes { get; set; }
  public DateTime? ReviewedAt { get; set; }
  public Guid? ReviewedBy { get; set; }

  public Job? Job { get; set; }
  public Candidate? Candidate { get; set; }
  public StatusType? Status { get; set; }
}
