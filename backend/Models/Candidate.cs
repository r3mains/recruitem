namespace backend.Models;

public class Candidate
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public string? ContactNumber { get; set; }
  public Guid? AddressId { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public bool IsDeleted { get; set; } = false;

  // Navigation properties
  public User User { get; set; } = null!;
  public Address? Address { get; set; }
  public ICollection<Document> Documents { get; set; } = new List<Document>();
  public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
  public ICollection<Verification> Verifications { get; set; } = new List<Verification>();
  public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
  public ICollection<CandidateQualification> CandidateQualifications { get; set; } = new List<CandidateQualification>();
}
