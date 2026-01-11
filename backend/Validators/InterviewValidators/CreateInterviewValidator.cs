using FluentValidation;
using backend.DTOs.Interview;

namespace backend.Validators;

public class CreateInterviewValidator : AbstractValidator<CreateInterviewDto>
{
  public CreateInterviewValidator()
  {
    RuleFor(x => x.JobApplicationId)
      .NotEmpty()
      .WithMessage("Job application is required");

    RuleFor(x => x.InterviewTypeId)
      .NotEmpty()
      .WithMessage("Interview type is required");

    RuleFor(x => x.InterviewerIds)
      .NotNull()
      .WithMessage("Interviewer list is required");
  }
}
