using FluentValidation;
using backend.DTOs.Location;

namespace backend.Validators.LocationValidators;

public class CreateStateValidator : AbstractValidator<CreateStateDto>
{
  public CreateStateValidator()
  {
    RuleFor(x => x.StateName)
      .NotEmpty()
      .WithMessage("State name is required.")
      .MaximumLength(100)
      .WithMessage("State name must not exceed 100 characters.");

    RuleFor(x => x.CountryId)
      .NotEmpty()
      .WithMessage("Country ID is required.");
  }
}

public class UpdateStateValidator : AbstractValidator<UpdateStateDto>
{
  public UpdateStateValidator()
  {
    RuleFor(x => x.StateName)
      .NotEmpty()
      .WithMessage("State name is required.")
      .MaximumLength(100)
      .WithMessage("State name must not exceed 100 characters.");

    RuleFor(x => x.CountryId)
      .NotEmpty()
      .WithMessage("Country ID is required.");
  }
}
