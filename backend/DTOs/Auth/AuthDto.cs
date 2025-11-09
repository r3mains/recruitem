namespace backend.DTOs;

// Authentication Request DTOs
public record RegisterDto(
  string Email,
  string Password
);

public record LoginDto(
  string Email,
  string Password
);

public record ForgotPasswordDto(
  string Email
);

public record ResetPasswordDto(
  string Email,
  string Token,
  string NewPassword
);

public record ChangePasswordDto(
  string CurrentPassword,
  string NewPassword
);

public record ConfirmEmailDto(
  string UserId,
  string Token
);

public record ResendConfirmationDto(
  string Email
);

// Authentication Response DTOs
public record AuthResponseDto(
  Guid Id,
  string? Email,
  string? UserName,
  string Token
);

public record LoginResponseDto(
  string AccessToken,
  IList<string> Roles
);

public record UserProfileDto(
  Guid Id,
  string? Email,
  string? UserName,
  IList<string> Roles,
  bool EmailConfirmed,
  DateTime? CreatedAt
);
