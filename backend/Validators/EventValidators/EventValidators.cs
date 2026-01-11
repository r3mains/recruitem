using FluentValidation;
using backend.DTOs.Event;

namespace backend.Validators.EventValidators;

public class CreateEventValidator : AbstractValidator<CreateEventDto>
{
  public CreateEventValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Event name is required.")
      .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.");

    RuleFor(x => x.Type)
      .MaximumLength(100).WithMessage("Event type must not exceed 100 characters.");

    RuleFor(x => x.Location)
      .MaximumLength(500).WithMessage("Location must not exceed 500 characters.");

    RuleFor(x => x.Date)
      .NotEmpty().WithMessage("Event date is required.")
      .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future.");

    RuleFor(x => x.CreatedBy)
      .NotEmpty().WithMessage("Created by is required.");
  }
}

public class UpdateEventValidator : AbstractValidator<UpdateEventDto>
{
  public UpdateEventValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Event name is required.")
      .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.");

    RuleFor(x => x.Type)
      .MaximumLength(100).WithMessage("Event type must not exceed 100 characters.");

    RuleFor(x => x.Location)
      .MaximumLength(500).WithMessage("Location must not exceed 500 characters.");

    RuleFor(x => x.Date)
      .NotEmpty().WithMessage("Event date is required.");
  }
}

public class RegisterCandidateToEventValidator : AbstractValidator<RegisterCandidateToEventDto>
{
  public RegisterCandidateToEventValidator()
  {
    RuleFor(x => x.EventId)
      .NotEmpty().WithMessage("Event ID is required.");

    RuleFor(x => x.CandidateId)
      .NotEmpty().WithMessage("Candidate ID is required.");

    RuleFor(x => x.StatusId)
      .NotEmpty().WithMessage("Status ID is required.");
  }
}

public class UpdateEventCandidateStatusValidator : AbstractValidator<UpdateEventCandidateStatusDto>
{
  public UpdateEventCandidateStatusValidator()
  {
    RuleFor(x => x.StatusId)
      .NotEmpty().WithMessage("Status ID is required.");
  }
}
