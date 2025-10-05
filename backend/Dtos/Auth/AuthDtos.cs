namespace Backend.Dtos.Auth;

public class LoginRequestDto
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public Guid RoleId { get; set; }
}

public class AuthResponseDto
{
  public string AccessToken { get; set; } = string.Empty;
  public DateTime ExpiresAtUtc { get; set; }
  public string TokenType { get; set; } = "Bearer";
}
