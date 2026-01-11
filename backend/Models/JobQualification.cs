namespace backend.Models;

public class JobQualification
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid JobId { get; set; }
  public Guid QualificationId { get; set; }
  public double? MinRequired { get; set; }


  public virtual Job Job { get; set; } = null!;
  public virtual Qualification Qualification { get; set; } = null!;
}
