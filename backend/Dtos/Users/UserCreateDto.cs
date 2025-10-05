namespace Backend.Dtos.Users;

public class UserCreateDto
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public Guid RoleId { get; set; }
}
