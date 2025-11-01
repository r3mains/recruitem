namespace Backend.Dtos.Users;

public class UserDto
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public Guid RoleId { get; set; }
  public string? RoleName { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
