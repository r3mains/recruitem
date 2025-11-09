using FluentValidation;
using backend.DTOs.Qualification;

namespace backend.Validators.QualificationValidators;

public class CreateQualificationValidator : AbstractValidator<CreateQualificationDto>
{
  public CreateQualificationValidator()
  {
    RuleFor(x => x.QualificationName)
        .NotEmpty().WithMessage("Qualification name is required.")
        .Length(2, 100).WithMessage("Qualification name must be between 2 and 100 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-\.,()&]+$").WithMessage("Qualification name contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.");
  }
}

public class UpdateQualificationValidator : AbstractValidator<UpdateQualificationDto>
{
  public UpdateQualificationValidator()
  {
    RuleFor(x => x.QualificationName)
        .NotEmpty().WithMessage("Qualification name is required.")
        .Length(2, 100).WithMessage("Qualification name must be between 2 and 100 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-\.,()&]+$").WithMessage("Qualification name contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.");
  }
}
