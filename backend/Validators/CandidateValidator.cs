using Backend.Models;
using FluentValidation;

namespace Backend.Validators;

public class CandidateValidator : AbstractValidator<Candidate>
{
  public CandidateValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty().WithMessage("User is required.");

    When(x => !string.IsNullOrWhiteSpace(x.FullName), () =>
    {
      RuleFor(x => x.FullName!)
        .MaximumLength(200).WithMessage("Full name must be 200 characters or fewer.");
    });

    When(x => !string.IsNullOrWhiteSpace(x.ContactNumber), () =>
    {
      RuleFor(x => x.ContactNumber!)
        .MaximumLength(50).WithMessage("Contact number must be 50 characters or fewer.");
    });

    When(x => !string.IsNullOrWhiteSpace(x.ResumeUrl), () =>
    {
      RuleFor(x => x.ResumeUrl!)
        .MaximumLength(500).WithMessage("Resume URL must be 500 characters or fewer.");
    });
  }
}
