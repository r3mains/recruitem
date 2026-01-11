using FluentValidation;
using backend.DTOs.JobApplication;

namespace backend.Validators.JobApplicationValidators;

public class CreateJobApplicationValidator : AbstractValidator<CreateJobApplicationDto>
{
  public CreateJobApplicationValidator()
  {
    RuleFor(x => x.JobId)
      .NotEmpty().WithMessage("Job ID is required");

    RuleFor(x => x.CandidateId)
      .NotEmpty().WithMessage("Candidate ID is required");
  }
}

public class UpdateJobApplicationValidator : AbstractValidator<UpdateJobApplicationDto>
{
  public UpdateJobApplicationValidator()
  {
    RuleFor(x => x.StatusId)
      .NotEmpty().WithMessage("Status ID is required");

    RuleFor(x => x.Score)
      .GreaterThanOrEqualTo(0).WithMessage("Score cannot be negative")
      .LessThanOrEqualTo(100).WithMessage("Score cannot exceed 100")
      .When(x => x.Score.HasValue);

    RuleFor(x => x.Comment)
      .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters")
      .When(x => !string.IsNullOrEmpty(x.Comment));
  }
}

public class ApplyToJobValidator : AbstractValidator<ApplyToJobDto>
{
  public ApplyToJobValidator()
  {
    RuleFor(x => x.JobId)
      .NotEmpty().WithMessage("Job ID is required");
  }
}

public class BulkApplicationActionValidator : AbstractValidator<BulkApplicationActionDto>
{
  public BulkApplicationActionValidator()
  {
    RuleFor(x => x.ApplicationIds)
      .NotEmpty().WithMessage("At least one application ID is required")
      .Must(x => x.All(id => id != Guid.Empty)).WithMessage("All application IDs must be valid");

    RuleFor(x => x.StatusId)
      .NotEmpty().WithMessage("Status ID is required");

    RuleFor(x => x.Comment)
      .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters")
      .When(x => !string.IsNullOrEmpty(x.Comment));
  }
}
