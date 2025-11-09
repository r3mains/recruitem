using FluentValidation;
using backend.DTOs.Location;

namespace backend.Validators.LocationValidators;

public class CreateCountryValidator : AbstractValidator<CreateCountryDto>
{
  public CreateCountryValidator()
  {
    RuleFor(x => x.CountryName)
      .NotEmpty()
      .WithMessage("Country name is required.")
      .MaximumLength(100)
      .WithMessage("Country name must not exceed 100 characters.");
  }
}

public class UpdateCountryValidator : AbstractValidator<UpdateCountryDto>
{
  public UpdateCountryValidator()
  {
    RuleFor(x => x.CountryName)
      .NotEmpty()
      .WithMessage("Country name is required.")
      .MaximumLength(100)
      .WithMessage("Country name must not exceed 100 characters.");
  }
}
