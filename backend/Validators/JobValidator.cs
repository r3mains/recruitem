using Backend.Dtos.Jobs;
using FluentValidation;

namespace Backend.Validators;

public class JobCreateValidator : AbstractValidator<JobCreateDto>
{
  public JobCreateValidator()
  {
    RuleFor(x => x.RecruiterId).NotEmpty().WithMessage("RecruiterId is required");
    RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
    RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
    RuleFor(x => x.JobTypeId).NotEmpty().WithMessage("JobTypeId is required");
    RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
    RuleFor(x => x.PositionId).NotEmpty().WithMessage("PositionId is required");
    RuleFor(x => x.StatusId).NotEmpty().WithMessage("StatusId is required");
    RuleFor(x => x).Must(x => !(x.SalaryMin.HasValue && x.SalaryMax.HasValue) || x.SalaryMin <= x.SalaryMax)
      .WithMessage("salary_min must be <= salary_max");
  }
}

public class JobUpdateValidator : AbstractValidator<JobUpdateDto>
{
  public JobUpdateValidator()
  {
    RuleFor(x => x).Must(x => x.Title != null || x.Description != null || x.JobTypeId.HasValue || x.Location.HasValue || x.SalaryMin.HasValue || x.SalaryMax.HasValue || x.PositionId.HasValue || x.StatusId.HasValue || x.ClosedReason != null)
      .WithMessage("Provide at least one field to update");
    RuleFor(x => x).Must(x => !(x.SalaryMin.HasValue && x.SalaryMax.HasValue) || x.SalaryMin <= x.SalaryMax)
      .WithMessage("salary_min must be <= salary_max");
  }
}
