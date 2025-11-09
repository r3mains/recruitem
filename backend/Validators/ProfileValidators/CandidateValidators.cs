using FluentValidation;
using backend.DTOs.Profile;

namespace backend.Validators.ProfileValidators;

public class CreateCandidateValidator : AbstractValidator<CreateCandidateDto>
{
  public CreateCandidateValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("User ID is required.");

    RuleFor(x => x.FullName)
      .MaximumLength(255)
      .WithMessage("Full name must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.FullName));

    RuleFor(x => x.ContactNumber)
      .MaximumLength(20)
      .WithMessage("Contact number must not exceed 20 characters.")
      .Matches(@"^[\+]?[\s\d\-\(\)]*$")
      .WithMessage("Contact number contains invalid characters.")
      .When(x => !string.IsNullOrEmpty(x.ContactNumber));
  }
}

public class UpdateCandidateValidator : AbstractValidator<UpdateCandidateDto>
{
  public UpdateCandidateValidator()
  {
    RuleFor(x => x.FullName)
      .MaximumLength(255)
      .WithMessage("Full name must not exceed 255 characters.")
      .When(x => !string.IsNullOrEmpty(x.FullName));

    RuleFor(x => x.ContactNumber)
      .MaximumLength(20)
      .WithMessage("Contact number must not exceed 20 characters.")
      .Matches(@"^[\+]?[\s\d\-\(\)]*$")
      .WithMessage("Contact number contains invalid characters.")
      .When(x => !string.IsNullOrEmpty(x.ContactNumber));
  }
}
