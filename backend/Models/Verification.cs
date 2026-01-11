using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Verification
  {
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public Guid StatusId { get; set; }
    public VerificationStatus Status { get; set; } = null!;

    public string? Comments { get; set; }

    public Guid? VerifiedBy { get; set; }
    public Employee? VerifiedByEmployee { get; set; }

    public DateTime? VerifiedAt { get; set; }
  }
}
