using FluentValidation;
using backend.DTOs.Location;

namespace backend.Validators.LocationValidators;

public class CreateCityValidator : AbstractValidator<CreateCityDto>
{
  public CreateCityValidator()
  {
    RuleFor(x => x.CityName)
      .NotEmpty()
      .WithMessage("City name is required.")
      .MaximumLength(100)
      .WithMessage("City name must not exceed 100 characters.");

    RuleFor(x => x.StateId)
      .NotEmpty()
      .WithMessage("State ID is required.");
  }
}

public class UpdateCityValidator : AbstractValidator<UpdateCityDto>
{
  public UpdateCityValidator()
  {
    RuleFor(x => x.CityName)
      .NotEmpty()
      .WithMessage("City name is required.")
      .MaximumLength(100)
      .WithMessage("City name must not exceed 100 characters.");

    RuleFor(x => x.StateId)
      .NotEmpty()
      .WithMessage("State ID is required.");
  }
}
