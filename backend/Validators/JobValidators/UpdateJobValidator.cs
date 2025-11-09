using FluentValidation;
using backend.DTOs.Job;

namespace backend.Validators.JobValidators
{
  public class UpdateJobValidator : AbstractValidator<UpdateJobDto>
  {
    public UpdateJobValidator()
    {
      RuleFor(x => x.Title)
          .Length(1, 200).WithMessage("Title must be between 1 and 200 characters")
          .Matches(@"^[a-zA-Z0-9\s\-\.,/()&]+$").WithMessage("Title contains invalid characters. Only letters, numbers, spaces, and common symbols are allowed.")
          .When(x => !string.IsNullOrEmpty(x.Title));

      RuleFor(x => x.Description)
          .Length(10, 5000).WithMessage("Description must be between 10 and 5000 characters")
          .When(x => !string.IsNullOrEmpty(x.Description));

      RuleFor(x => x.JobTypeId)
          .NotEqual(Guid.Empty).WithMessage("Job type ID cannot be empty")
          .When(x => x.JobTypeId.HasValue);

      RuleFor(x => x.AddressId)
          .NotEqual(Guid.Empty).WithMessage("Address ID cannot be empty")
          .When(x => x.AddressId.HasValue);

      RuleFor(x => x.StatusId)
          .NotEqual(Guid.Empty).WithMessage("Status ID cannot be empty")
          .When(x => x.StatusId.HasValue);

      RuleFor(x => x.SalaryMin)
          .GreaterThanOrEqualTo(0).WithMessage("Minimum salary must be positive")
          .When(x => x.SalaryMin.HasValue);

      RuleFor(x => x.SalaryMax)
          .GreaterThanOrEqualTo(0).WithMessage("Maximum salary must be positive")
          .When(x => x.SalaryMax.HasValue);

      RuleFor(x => x)
          .Must(x => !x.SalaryMin.HasValue || !x.SalaryMax.HasValue || x.SalaryMin <= x.SalaryMax)
          .WithMessage("Minimum salary cannot be greater than maximum salary");

      RuleFor(x => x.ClosedReason)
          .MaximumLength(500).WithMessage("Closed reason cannot exceed 500 characters")
          .When(x => !string.IsNullOrEmpty(x.ClosedReason));

      RuleForEach(x => x.RequiredSkillIds)
          .NotEqual(Guid.Empty).WithMessage("Required skill ID cannot be empty")
          .When(x => x.RequiredSkillIds != null);

      RuleForEach(x => x.PreferredSkillIds)
          .NotEqual(Guid.Empty).WithMessage("Preferred skill ID cannot be empty")
          .When(x => x.PreferredSkillIds != null);

      RuleForEach(x => x.Qualifications)
          .SetValidator(new JobQualificationValidator())
          .When(x => x.Qualifications != null);
    }
  }
}
