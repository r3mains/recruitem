using FluentValidation;
using backend.DTOs.Candidate;

namespace backend.Validators.CandidateValidators;

public class CreateCandidateValidator : AbstractValidator<CreateCandidateDto>
{
  public CreateCandidateValidator()
  {
    RuleFor(x => x.FullName)
      .NotEmpty().WithMessage("Full name is required")
      .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
      .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format")
      .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(6).WithMessage("Password must be at least 6 characters")
      .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

    RuleFor(x => x.ContactNumber)
      .MaximumLength(20).WithMessage("Contact number cannot exceed 20 characters")
      .When(x => !string.IsNullOrEmpty(x.ContactNumber));

    When(x => x.Address != null, () =>
    {
      RuleFor(x => x.Address!.CityId)
        .NotEmpty().WithMessage("City is required when address is provided");

      RuleFor(x => x.Address!.Street)
        .MaximumLength(200).WithMessage("Street cannot exceed 200 characters")
        .When(x => !string.IsNullOrEmpty(x.Address?.Street));

      RuleFor(x => x.Address!.PostalCode)
        .MaximumLength(10).WithMessage("Postal code cannot exceed 10 characters")
        .When(x => !string.IsNullOrEmpty(x.Address?.PostalCode));
    });

    RuleForEach(x => x.Skills).SetValidator(new CreateCandidateSkillValidator())
      .When(x => x.Skills != null && x.Skills.Any());

    RuleForEach(x => x.Qualifications).SetValidator(new CreateCandidateQualificationValidator())
      .When(x => x.Qualifications != null && x.Qualifications.Any());
  }
}

public class UpdateCandidateValidator : AbstractValidator<UpdateCandidateDto>
{
  public UpdateCandidateValidator()
  {
    RuleFor(x => x.FullName)
      .NotEmpty().WithMessage("Full name is required")
      .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
      .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

    RuleFor(x => x.ContactNumber)
      .MaximumLength(20).WithMessage("Contact number cannot exceed 20 characters")
      .When(x => !string.IsNullOrEmpty(x.ContactNumber));

    When(x => x.Address != null, () =>
    {
      RuleFor(x => x.Address!.CityId)
        .NotEmpty().WithMessage("City is required when address is provided");

      RuleFor(x => x.Address!.Street)
        .MaximumLength(200).WithMessage("Street cannot exceed 200 characters")
        .When(x => !string.IsNullOrEmpty(x.Address?.Street));

      RuleFor(x => x.Address!.PostalCode)
        .MaximumLength(10).WithMessage("Postal code cannot exceed 10 characters")
        .When(x => !string.IsNullOrEmpty(x.Address?.PostalCode));
    });

    RuleForEach(x => x.Skills).SetValidator(new CreateCandidateSkillValidator())
      .When(x => x.Skills != null && x.Skills.Any());

    RuleForEach(x => x.Qualifications).SetValidator(new CreateCandidateQualificationValidator())
      .When(x => x.Qualifications != null && x.Qualifications.Any());
  }
}

public class CreateCandidateSkillValidator : AbstractValidator<CreateCandidateSkillDto>
{
  public CreateCandidateSkillValidator()
  {
    RuleFor(x => x.SkillId)
      .NotEmpty().WithMessage("Skill ID is required");

    RuleFor(x => x.YearOfExperience)
      .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative")
      .LessThanOrEqualTo(50).WithMessage("Years of experience cannot exceed 50")
      .When(x => x.YearOfExperience.HasValue);
  }
}

public class CreateCandidateQualificationValidator : AbstractValidator<CreateCandidateQualificationDto>
{
  public CreateCandidateQualificationValidator()
  {
    RuleFor(x => x.QualificationId)
      .NotEmpty().WithMessage("Qualification ID is required");

    RuleFor(x => x.CompletedOn)
      .LessThanOrEqualTo(DateTime.Now).WithMessage("Completion date cannot be in the future")
      .When(x => x.CompletedOn.HasValue);

    RuleFor(x => x.Grade)
      .GreaterThanOrEqualTo(0).WithMessage("Grade cannot be negative")
      .LessThanOrEqualTo(100).WithMessage("Grade cannot exceed 100")
      .When(x => x.Grade.HasValue);
  }
}
