using FluentValidation;
using backend.DTOs.Position;

namespace backend.Validators.PositionValidators;

public class UpdatePositionValidator : AbstractValidator<UpdatePositionDto>
{
  public UpdatePositionValidator()
  {
    RuleFor(x => x.Title)
        .Length(1, 200).WithMessage("Title must be between 1 and 200 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-\.,/()&]+$").WithMessage("Title contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.")
        .When(x => !string.IsNullOrEmpty(x.Title));

    RuleFor(x => x.NumberOfInterviews)
        .GreaterThanOrEqualTo(1).WithMessage("Number of interviews must be at least 1.")
        .LessThanOrEqualTo(10).WithMessage("Number of interviews cannot exceed 10.")
        .When(x => x.NumberOfInterviews.HasValue);

    RuleFor(x => x.StatusId)
        .NotEqual(Guid.Empty).WithMessage("Status ID cannot be empty.")
        .When(x => x.StatusId.HasValue);

    RuleFor(x => x.ReviewerId)
        .NotEqual(Guid.Empty).WithMessage("Reviewer ID cannot be empty.")
        .When(x => x.ReviewerId.HasValue);

    RuleFor(x => x.ClosedReason)
        .MaximumLength(500).WithMessage("Closed reason cannot exceed 500 characters.")
        .When(x => !string.IsNullOrEmpty(x.ClosedReason));

    RuleForEach(x => x.Skills)
        .SetValidator(new PositionSkillValidator())
        .When(x => x.Skills != null);
  }
}
