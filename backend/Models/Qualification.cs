namespace Backend.Models;

public class Qualification
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;

  public ICollection<CandidateQualification> CandidateQualifications { get; set; } = [];
}
