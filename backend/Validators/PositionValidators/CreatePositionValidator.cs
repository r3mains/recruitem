using FluentValidation;
using backend.DTOs.Position;

namespace backend.Validators.PositionValidators;

public class CreatePositionValidator : AbstractValidator<CreatePositionDto>
{
  public CreatePositionValidator()
  {
    RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Title is required.")
        .Length(1, 200).WithMessage("Title must be between 1 and 200 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-\.,/()&]+$").WithMessage("Title contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.");

    RuleFor(x => x.NumberOfInterviews)
        .GreaterThanOrEqualTo(1).WithMessage("Number of interviews must be at least 1.")
        .LessThanOrEqualTo(10).WithMessage("Number of interviews cannot exceed 10.");

    RuleFor(x => x.ReviewerId)
        .NotEqual(Guid.Empty).WithMessage("Reviewer ID cannot be empty.")
        .When(x => x.ReviewerId.HasValue);

    RuleFor(x => x.Skills)
        .NotNull().WithMessage("Skills list is required.");

    RuleForEach(x => x.Skills)
        .SetValidator(new PositionSkillValidator());
  }
}

public class PositionSkillValidator : AbstractValidator<PositionSkillDto>
{
  public PositionSkillValidator()
  {
    RuleFor(x => x.SkillId)
        .NotEqual(Guid.Empty).WithMessage("Skill ID is required and cannot be empty.");
  }
}
