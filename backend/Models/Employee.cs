namespace backend.Models;

public class Employee
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public Guid? BranchAddressId { get; set; }
  public DateOnly? JoiningDate { get; set; }
  public string? OfferLetterUrl { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public bool IsDeleted { get; set; } = false;

  // Navigation properties
  public User User { get; set; } = null!;
  public Address? BranchAddress { get; set; }
  public virtual ICollection<Position> ReviewedPositions { get; set; } = [];
  public virtual ICollection<Job> RecruitedJobs { get; set; } = [];
  public ICollection<Comment> Comments { get; set; } = new List<Comment>();
  public ICollection<Verification> Verifications { get; set; } = new List<Verification>();
}
