using FluentValidation;
using backend.DTOs.JobType;

namespace backend.Validators.JobTypeValidators;

public class CreateJobTypeValidator : AbstractValidator<CreateJobTypeDto>
{
  public CreateJobTypeValidator()
  {
    RuleFor(x => x.Type)
        .NotEmpty().WithMessage("Job type is required.")
        .Length(2, 50).WithMessage("Job type must be between 2 and 50 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("Job type can only contain letters, numbers, spaces, and hyphens.");
  }
}

public class UpdateJobTypeValidator : AbstractValidator<UpdateJobTypeDto>
{
  public UpdateJobTypeValidator()
  {
    RuleFor(x => x.Type)
        .NotEmpty().WithMessage("Job type is required.")
        .Length(2, 50).WithMessage("Job type must be between 2 and 50 characters.")
        .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("Job type can only contain letters, numbers, spaces, and hyphens.");
  }
}
