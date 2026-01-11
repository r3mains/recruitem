using FluentValidation;
using backend.DTOs.OfferLetter;

namespace backend.Validators;

public class CreateOfferLetterValidator : AbstractValidator<CreateOfferLetterDto>
{
  public CreateOfferLetterValidator()
  {
    RuleFor(x => x.JobApplicationId)
      .NotEmpty().WithMessage("Job application ID is required");

    RuleFor(x => x.JoiningDate)
      .NotEmpty().WithMessage("Joining date is required")
      .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Joining date must be in the future");

    RuleFor(x => x.Salary)
      .GreaterThan(0).WithMessage("Salary must be greater than 0");

    RuleFor(x => x.ExpiryDate)
      .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Expiry date must be in the future")
      .When(x => x.ExpiryDate.HasValue);
  }
}

public class UpdateOfferLetterValidator : AbstractValidator<UpdateOfferLetterDto>
{
  public UpdateOfferLetterValidator()
  {
    RuleFor(x => x.JoiningDate)
      .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Joining date must be in the future")
      .When(x => x.JoiningDate.HasValue);

    RuleFor(x => x.Salary)
      .GreaterThan(0).WithMessage("Salary must be greater than 0")
      .When(x => x.Salary.HasValue);

    RuleFor(x => x.ExpiryDate)
      .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Expiry date must be in the future")
      .When(x => x.ExpiryDate.HasValue);
  }
}

public class GenerateOfferLetterValidator : AbstractValidator<GenerateOfferLetterDto>
{
  public GenerateOfferLetterValidator()
  {
    RuleFor(x => x.OfferLetterId)
      .NotEmpty().WithMessage("Offer letter ID is required");
  }
}

public class RejectOfferValidator : AbstractValidator<RejectOfferDto>
{
  public RejectOfferValidator()
  {
    RuleFor(x => x.OfferLetterId)
      .NotEmpty().WithMessage("Offer letter ID is required");

    RuleFor(x => x.Reason)
      .NotEmpty().WithMessage("Rejection reason is required")
      .MaximumLength(1000).WithMessage("Reason cannot exceed 1000 characters");
  }
}
