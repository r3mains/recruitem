namespace Backend.Dtos.Candidates;

public class CandidateCreateDto
{
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public string? ContactNumber { get; set; }
  public string? ResumeUrl { get; set; }
  public Guid? AddressId { get; set; }
}
