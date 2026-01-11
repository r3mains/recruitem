namespace backend.Models;

public class Qualification
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string QualificationName { get; set; } = string.Empty;

  public virtual ICollection<JobQualification> JobQualifications { get; set; } = [];
  public virtual ICollection<CandidateQualification> CandidateQualifications { get; set; } = new List<CandidateQualification>();
}
