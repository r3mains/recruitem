using FluentValidation;
using backend.DTOs.Verification;

namespace backend.Validators.VerificationValidators;

public class CreateVerificationValidator : AbstractValidator<CreateVerificationDto>
{
  public CreateVerificationValidator()
  {
    RuleFor(x => x.CandidateId)
      .NotEmpty().WithMessage("Candidate ID is required.");

    RuleFor(x => x.DocumentId)
      .NotEmpty().WithMessage("Document ID is required.");

    RuleFor(x => x.StatusId)
      .NotEmpty().WithMessage("Status ID is required.");

    RuleFor(x => x.VerifiedBy)
      .NotEmpty().WithMessage("Verified by is required.");
  }
}

public class UpdateVerificationValidator : AbstractValidator<UpdateVerificationDto>
{
  public UpdateVerificationValidator()
  {
    RuleFor(x => x.StatusId)
      .NotEmpty().WithMessage("Status ID is required.");
  }
}
