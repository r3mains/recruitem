using FluentValidation;
using backend.DTOs.Job;

namespace backend.Validators.JobValidators
{
  public class CreateJobValidator : AbstractValidator<CreateJobDto>
  {
    public CreateJobValidator()
    {
      RuleFor(x => x.Title)
          .NotEmpty().WithMessage("Title is required")
          .Length(1, 200).WithMessage("Title must be between 1 and 200 characters")
          .Matches(@"^[a-zA-Z0-9\s\-\.,/()&]+$").WithMessage("Title contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.");

      RuleFor(x => x.Description)
          .NotEmpty().WithMessage("Description is required")
          .Length(10, 5000).WithMessage("Description must be between 10 and 5000 characters");

      RuleFor(x => x.JobTypeId)
          .NotEqual(Guid.Empty).WithMessage("Job type is required");

      RuleFor(x => x.AddressId)
          .NotEqual(Guid.Empty).WithMessage("Address is required");

      RuleFor(x => x.PositionId)
          .NotEqual(Guid.Empty).WithMessage("Position is required");

      RuleFor(x => x.SalaryMin)
          .GreaterThanOrEqualTo(0).WithMessage("Minimum salary must be positive")
          .When(x => x.SalaryMin.HasValue);

      RuleFor(x => x.SalaryMax)
          .GreaterThanOrEqualTo(0).WithMessage("Maximum salary must be positive")
          .When(x => x.SalaryMax.HasValue);

      RuleFor(x => x)
          .Must(x => !x.SalaryMin.HasValue || !x.SalaryMax.HasValue || x.SalaryMin <= x.SalaryMax)
          .WithMessage("Minimum salary cannot be greater than maximum salary");

      RuleForEach(x => x.RequiredSkillIds)
          .NotEqual(Guid.Empty).WithMessage("Required skill ID cannot be empty");

      RuleForEach(x => x.PreferredSkillIds)
          .NotEqual(Guid.Empty).WithMessage("Preferred skill ID cannot be empty");

      RuleForEach(x => x.Qualifications)
          .SetValidator(new JobQualificationValidator());
    }
  }

  public class JobQualificationValidator : AbstractValidator<JobQualificationDto>
  {
    public JobQualificationValidator()
    {
      RuleFor(x => x.QualificationId)
          .NotEqual(Guid.Empty).WithMessage("Qualification ID is required");

      RuleFor(x => x.MinRequired)
          .GreaterThanOrEqualTo(0).WithMessage("Minimum required score must be positive")
          .When(x => x.MinRequired.HasValue);
    }
  }
}
