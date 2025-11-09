namespace backend.DTOs.Qualification
{
  public class CreateQualificationDto
  {
    public string QualificationName { get; set; } = string.Empty;
  }

  public class UpdateQualificationDto
  {
    public string QualificationName { get; set; } = string.Empty;
  }

  public class QualificationResponseDto
  {
    public Guid Id { get; set; }
    public string QualificationName { get; set; } = string.Empty;

    public int JobCount { get; set; }
    public int CandidateCount { get; set; }
  }

  public class QualificationListDto
  {
    public Guid Id { get; set; }
    public string QualificationName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
  }
}
