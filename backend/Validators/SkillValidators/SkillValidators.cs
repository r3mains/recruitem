using FluentValidation;
using backend.DTOs.Skill;

namespace backend.Validators.SkillValidators;

public class CreateSkillValidator : AbstractValidator<CreateSkillDto>
{
  public CreateSkillValidator()
  {
    RuleFor(x => x.SkillName)
        .NotEmpty().WithMessage("Skill name is required.")
        .Length(2, 100).WithMessage("Skill name must be between 2 and 100 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-\+\.#/()&,]+$").WithMessage("Skill name contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.");
  }
}

public class UpdateSkillValidator : AbstractValidator<UpdateSkillDto>
{
  public UpdateSkillValidator()
  {
    RuleFor(x => x.SkillName)
        .Length(2, 100).WithMessage("Skill name must be between 2 and 100 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-\+\.#/()&,]+$").WithMessage("Skill name contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.")
        .When(x => !string.IsNullOrEmpty(x.SkillName));
  }
}
