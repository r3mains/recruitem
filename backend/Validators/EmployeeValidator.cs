using Backend.Models;
using FluentValidation;

namespace Backend.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
  public EmployeeValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty().WithMessage("User is required.");

    When(x => !string.IsNullOrWhiteSpace(x.FullName), () =>
    {
      RuleFor(x => x.FullName!)
        .MaximumLength(200).WithMessage("Full name must be 200 characters or fewer.");
    });

    When(x => !string.IsNullOrWhiteSpace(x.OfferLetterUrl), () =>
    {
      RuleFor(x => x.OfferLetterUrl!)
        .MaximumLength(500).WithMessage("Offer letter URL must be 500 characters or fewer.");
    });
  }
}
