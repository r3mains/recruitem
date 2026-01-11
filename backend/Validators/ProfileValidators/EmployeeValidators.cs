using FluentValidation;
using backend.DTOs.Profile;

namespace backend.Validators.ProfileValidators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
  public CreateEmployeeValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("User ID is required.");

    RuleFor(x => x.FullName)
      .MaximumLength(255)
      .WithMessage("Full name must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.FullName));

    RuleFor(x => x.OfferLetterUrl)
      .MaximumLength(500)
      .WithMessage("Offer letter URL must not exceed 500 characters.")
      .Must(BeAValidUrl)
      .WithMessage("Offer letter URL must be a valid URL.")
      .When(x => !string.IsNullOrEmpty(x.OfferLetterUrl));

    RuleFor(x => x.JoiningDate)
      .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
      .WithMessage("Joining date cannot be in the future.")
      .When(x => x.JoiningDate.HasValue);
  }

  private static bool BeAValidUrl(string? url)
  {
    if (string.IsNullOrEmpty(url)) return true;
    return Uri.TryCreate(url, UriKind.Absolute, out _);
  }
}

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
{
  public UpdateEmployeeValidator()
  {
    RuleFor(x => x.FullName)
      .MaximumLength(255)
      .WithMessage("Full name must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.FullName));

    RuleFor(x => x.OfferLetterUrl)
      .MaximumLength(500)
      .WithMessage("Offer letter URL must not exceed 500 characters.")
      .Must(BeAValidUrl)
      .WithMessage("Offer letter URL must be a valid URL.")
      .When(x => !string.IsNullOrEmpty(x.OfferLetterUrl));

    RuleFor(x => x.JoiningDate)
      .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
      .WithMessage("Joining date cannot be in the future.")
      .When(x => x.JoiningDate.HasValue);
  }

  private static bool BeAValidUrl(string? url)
  {
    if (string.IsNullOrEmpty(url)) return true;
    return Uri.TryCreate(url, UriKind.Absolute, out _);
  }
}
