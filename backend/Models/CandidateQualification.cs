namespace Backend.Models;

public class CandidateQualification
{
  public Guid Id { get; set; }
  public Guid CandidateId { get; set; }
  public Guid QualificationId { get; set; }
  public DateTime? YearCompleted { get; set; }
  public double? Grade { get; set; }

  public Candidate? Candidate { get; set; }
  public Qualification? Qualification { get; set; }
}
