using FluentValidation;
using backend.DTOs.Screening;

namespace backend.Validators.ScreeningValidators;

public class ScreenResumeValidator : AbstractValidator<ScreenResumeDto>
{
  public ScreenResumeValidator()
  {
    RuleFor(x => x.JobApplicationId)
      .NotEmpty().WithMessage("Job application ID is required");

    RuleFor(x => x.Score)
      .GreaterThanOrEqualTo(0).WithMessage("Score cannot be negative")
      .LessThanOrEqualTo(100).WithMessage("Score cannot exceed 100")
      .When(x => x.Score.HasValue);

    RuleFor(x => x.Comments)
      .MaximumLength(1000).WithMessage("Comments cannot exceed 1000 characters")
      .When(x => !string.IsNullOrEmpty(x.Comments));
  }
}

public class AddCommentValidator : AbstractValidator<AddCommentDto>
{
  public AddCommentValidator()
  {
    RuleFor(x => x.JobApplicationId)
      .NotEmpty().WithMessage("Job application ID is required");

    RuleFor(x => x.Comment)
      .NotEmpty().WithMessage("Comment is required")
      .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
  }
}

public class UpdateCandidateSkillsValidator : AbstractValidator<UpdateCandidateSkillsDto>
{
  public UpdateCandidateSkillsValidator()
  {
    RuleFor(x => x.Skills)
      .NotNull().WithMessage("Skills list is required")
      .Must(x => x.Any()).WithMessage("At least one skill is required");

    RuleForEach(x => x.Skills).SetValidator(new CandidateSkillUpdateValidator());
  }
}

public class CandidateSkillUpdateValidator : AbstractValidator<CandidateSkillUpdateDto>
{
  public CandidateSkillUpdateValidator()
  {
    RuleFor(x => x.SkillId)
      .NotEmpty().WithMessage("Skill ID is required");

    RuleFor(x => x.YearsOfExperience)
      .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative")
      .LessThanOrEqualTo(50).WithMessage("Years of experience cannot exceed 50")
      .When(x => x.YearsOfExperience.HasValue);

    RuleFor(x => x.ProficiencyLevel)
      .Must(x => string.IsNullOrEmpty(x) || new[] { "Beginner", "Intermediate", "Advanced", "Expert" }.Contains(x))
      .WithMessage("Proficiency level must be one of: Beginner, Intermediate, Advanced, Expert")
      .When(x => !string.IsNullOrEmpty(x.ProficiencyLevel));
  }
}

public class AssignReviewerValidator : AbstractValidator<AssignReviewerDto>
{
  public AssignReviewerValidator()
  {
    RuleFor(x => x.PositionId)
      .NotEmpty().WithMessage("Position ID is required");

    RuleFor(x => x.ReviewerId)
      .NotEmpty().WithMessage("Reviewer ID is required");
  }
}

public class ShortlistCandidateValidator : AbstractValidator<ShortlistCandidateDto>
{
  public ShortlistCandidateValidator()
  {
    RuleFor(x => x.JobApplicationId)
      .NotEmpty().WithMessage("Job application ID is required");

    RuleFor(x => x.Comments)
      .MaximumLength(1000).WithMessage("Comments cannot exceed 1000 characters")
      .When(x => !string.IsNullOrEmpty(x.Comments));
  }
}
