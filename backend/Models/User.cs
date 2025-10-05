namespace Backend.Models;

public class User
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public Guid RoleId { get; set; }
  public Role? Role { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public Candidate? Candidate { get; set; }
  public Employee? Employee { get; set; }
}
