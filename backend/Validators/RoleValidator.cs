using Backend.Models;
using FluentValidation;

namespace Backend.Validators;

public class RoleValidator : AbstractValidator<Role>
{
  public RoleValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Role name is required.")
      .MaximumLength(100).WithMessage("Role name must be 100 characters or fewer.");
  }
}
