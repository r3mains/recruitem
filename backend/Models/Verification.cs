using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Verification
  {
    public Guid Id { get; set; }

    [Required]
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    [Required]
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    [Required]
    public Guid StatusId { get; set; }
    public VerificationStatus Status { get; set; } = null!;

    public string? Comments { get; set; }

    [Required]
    public Guid VerifiedBy { get; set; }
    public Employee VerifiedByEmployee { get; set; } = null!;

    public DateTime? VerifiedAt { get; set; }
  }
}
