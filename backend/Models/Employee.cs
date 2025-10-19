namespace Backend.Models;

public class Employee
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public Guid? BranchAddressId { get; set; }
  public DateOnly? JoiningDate { get; set; }
  public string? OfferLetterUrl { get; set; }

  public User? User { get; set; }
  public Address? BranchAddress { get; set; }
  public ICollection<Job> RecruitedJobs { get; set; } = [];
  public ICollection<Position> ReviewedPositions { get; set; } = [];

  public string? FirstName => FullName?.Split(' ').FirstOrDefault();
  public string? LastName => FullName?.Split(' ').Skip(1).FirstOrDefault();
}
