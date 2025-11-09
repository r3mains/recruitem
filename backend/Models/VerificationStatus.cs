namespace backend.Models;

public class VerificationStatus
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Status { get; set; } = string.Empty;

  // Navigation properties
  public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();
}
