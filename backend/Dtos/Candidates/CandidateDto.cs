namespace Backend.Dtos.Candidates;

public class CandidateDto
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public string? ContactNumber { get; set; }
  public string? ResumeUrl { get; set; }
  public Guid? AddressId { get; set; }
  public string? Email { get; set; }
  public string? AddressDetails { get; set; }
  public List<CandidateSkillDto>? Skills { get; set; }
  public int TotalApplications { get; set; }
}

public class CandidateSearchResultDto
{
  public List<CandidateDto> Candidates { get; set; } = [];
  public int TotalCount { get; set; }
  public int Page { get; set; }
  public int Limit { get; set; }
  public int TotalPages { get; set; }
}
