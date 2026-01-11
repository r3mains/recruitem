using FluentValidation;
using backend.DTOs.Interview;

namespace backend.Validators;

public class CreateInterviewScheduleValidator : AbstractValidator<CreateInterviewScheduleDto>
{
  public CreateInterviewScheduleValidator()
  {
    RuleFor(x => x.InterviewId)
      .NotEmpty()
      .WithMessage("Interview is required");

    RuleFor(x => x.ScheduledAt)
      .NotEmpty()
      .WithMessage("Interview date and time is required")
      .GreaterThan(DateTime.UtcNow)
      .WithMessage("Interview date must be in the future");

    RuleFor(x => x.Location)
      .MaximumLength(500)
      .WithMessage("Location cannot exceed 500 characters");

    RuleFor(x => x.MeetingLink)
      .MaximumLength(1000)
      .WithMessage("Meeting link cannot exceed 1000 characters");
  }
}
