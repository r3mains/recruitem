using FluentValidation;
using backend.DTOs.Interview;

namespace backend.Validators;

public class CreateInterviewFeedbackValidator : AbstractValidator<CreateInterviewFeedbackDto>
{
  public CreateInterviewFeedbackValidator()
  {
    RuleFor(x => x.InterviewId)
      .NotEmpty()
      .WithMessage("Interview is required");

    RuleFor(x => x.ForSkill)
      .NotEmpty()
      .WithMessage("Skill is required");

    RuleFor(x => x.Rating)
      .NotEmpty()
      .WithMessage("Rating is required")
      .InclusiveBetween(1, 5)
      .WithMessage("Rating must be between 1 and 5");

    RuleFor(x => x.Feedback)
      .MaximumLength(2000)
      .WithMessage("Feedback cannot exceed 2000 characters");
  }
}
