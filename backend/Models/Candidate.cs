namespace Backend.Models;

public class Candidate
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public User? User { get; set; }
  public string? FullName { get; set; }
  public string? ContactNumber { get; set; }
  public string? ResumeUrl { get; set; }
  public Guid? AddressId { get; set; }
}
