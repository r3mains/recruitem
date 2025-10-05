namespace Backend.Dtos.Users;

public class UserUpdateDto
{
  public string? Email { get; set; }
  public string? Password { get; set; }
  public Guid? RoleId { get; set; }
}
