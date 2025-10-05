using Backend.Models;
using FluentValidation;

namespace Backend.Validators;

public class UserValidator : AbstractValidator<User>
{
  public UserValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Email is invalid.")
      .MaximumLength(256).WithMessage("Email must be 256 characters or fewer.");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required.")
      .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

    RuleFor(x => x.RoleId)
      .NotEmpty().WithMessage("Role is required.");
  }
}
