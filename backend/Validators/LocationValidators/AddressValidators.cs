using FluentValidation;
using backend.DTOs.Location;

namespace backend.Validators.LocationValidators;

public class CreateAddressValidator : AbstractValidator<CreateAddressDto>
{
  public CreateAddressValidator()
  {
    RuleFor(x => x.AddressLine1)
      .MaximumLength(255)
      .WithMessage("Address line 1 must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.AddressLine1));

    RuleFor(x => x.AddressLine2)
      .MaximumLength(255)
      .WithMessage("Address line 2 must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.AddressLine2));

    RuleFor(x => x.Locality)
      .MaximumLength(100)
      .WithMessage("Locality must not exceed 100 characters.")
      .When(x => !string.IsNullOrEmpty(x.Locality));

    RuleFor(x => x.Pincode)
      .MaximumLength(20)
      .WithMessage("Pincode must not exceed 20 characters.")
      .When(x => !string.IsNullOrEmpty(x.Pincode));
  }
}

public class UpdateAddressValidator : AbstractValidator<UpdateAddressDto>
{
  public UpdateAddressValidator()
  {
    RuleFor(x => x.AddressLine1)
      .MaximumLength(255)
      .WithMessage("Address line 1 must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.AddressLine1));

    RuleFor(x => x.AddressLine2)
      .MaximumLength(255)
      .WithMessage("Address line 2 must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.AddressLine2));

    RuleFor(x => x.Locality)
      .MaximumLength(100)
      .WithMessage("Locality must not exceed 100 characters.")
      .When(x => !string.IsNullOrEmpty(x.Locality));

    RuleFor(x => x.Pincode)
      .MaximumLength(20)
      .WithMessage("Pincode must not exceed 20 characters.")
      .When(x => !string.IsNullOrEmpty(x.Pincode));
  }
}
