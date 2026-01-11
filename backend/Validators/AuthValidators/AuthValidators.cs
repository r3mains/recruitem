using backend.DTOs;
using FluentValidation;

namespace backend.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
  public RegisterValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email address.");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required.")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
      .Must(HaveDigit)
      .WithMessage("Passwords must have at least one digit ('0'-'9').")
      .Must(HaveUniqueChars)
      .WithMessage("Passwords must have at least one unique character.")
      .Must(HaveNonAlphanumericCharacter)
      .WithMessage("Passwords must have at least one non alphanumeric character.")
      .Must(HaveLowercase)
      .WithMessage("Passwords must have at least one lowercase ('a'-'z').")
      .Must(HaveUppercase)
      .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
  }

  private static bool HaveDigit(string password)
  {
    return password.Any(c => char.IsDigit(c));
  }

  private static bool HaveUniqueChars(string password)
  {
    return password.Distinct().Count() >= 1;
  }

  private static bool HaveNonAlphanumericCharacter(string password)
  {
    return password.Any(c => !char.IsLetterOrDigit(c));
  }

  private static bool HaveLowercase(string password)
  {
    return password.Any(c => char.IsLower(c));
  }

  private static bool HaveUppercase(string password)
  {
    return password.Any(c => char.IsUpper(c));
  }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
  public LoginValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email address.");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required.");
  }
}

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
{
  public ForgotPasswordValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email address.");
  }
}

public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
{
  public ResetPasswordValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email address.");

    RuleFor(x => x.Token)
      .NotEmpty().WithMessage("Reset token is required.");

    RuleFor(x => x.NewPassword)
      .NotEmpty().WithMessage("New password is required.")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
      .Must(HaveDigit)
      .WithMessage("Passwords must have at least one digit ('0'-'9').")
      .Must(HaveNonAlphanumericCharacter)
      .WithMessage("Passwords must have at least one non alphanumeric character.")
      .Must(HaveLowercase)
      .WithMessage("Passwords must have at least one lowercase ('a'-'z').")
      .Must(HaveUppercase)
      .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
  }

  private static bool HaveDigit(string password)
  {
    return password.Any(c => char.IsDigit(c));
  }

  private static bool HaveNonAlphanumericCharacter(string password)
  {
    return password.Any(c => !char.IsLetterOrDigit(c));
  }

  private static bool HaveLowercase(string password)
  {
    return password.Any(c => char.IsLower(c));
  }

  private static bool HaveUppercase(string password)
  {
    return password.Any(c => char.IsUpper(c));
  }
}

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
  public ChangePasswordValidator()
  {
    RuleFor(x => x.CurrentPassword)
      .NotEmpty().WithMessage("Current password is required.");

    RuleFor(x => x.NewPassword)
      .NotEmpty().WithMessage("New password is required.")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
      .Must(HaveDigit)
      .WithMessage("Passwords must have at least one digit ('0'-'9').")
      .Must(HaveNonAlphanumericCharacter)
      .WithMessage("Passwords must have at least one non alphanumeric character.")
      .Must(HaveLowercase)
      .WithMessage("Passwords must have at least one lowercase ('a'-'z').")
      .Must(HaveUppercase)
      .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
  }

  private static bool HaveDigit(string password)
  {
    return password.Any(c => char.IsDigit(c));
  }

  private static bool HaveNonAlphanumericCharacter(string password)
  {
    return password.Any(c => !char.IsLetterOrDigit(c));
  }

  private static bool HaveLowercase(string password)
  {
    return password.Any(c => char.IsLower(c));
  }

  private static bool HaveUppercase(string password)
  {
    return password.Any(c => char.IsUpper(c));
  }
}

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailDto>
{
  public ConfirmEmailValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty().WithMessage("User ID is required.");

    RuleFor(x => x.Token)
      .NotEmpty().WithMessage("Confirmation token is required.");
  }
}

public class ResendConfirmationValidator : AbstractValidator<ResendConfirmationDto>
{
  public ResendConfirmationValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email address.");
  }
}
