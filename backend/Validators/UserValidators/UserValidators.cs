using backend.DTOs;
using backend.Consts;
using FluentValidation;

namespace backend.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
  public CreateUserValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email address.")
      .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required.")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
      .Must(HaveDigit)
      .WithMessage("Passwords must have at least one digit ('0'-'9').")
      .Must(HaveNonAlphanumericCharacter)
      .WithMessage("Passwords must have at least one non alphanumeric character.")
      .Must(HaveLowercase)
      .WithMessage("Passwords must have at least one lowercase ('a'-'z').")
      .Must(HaveUppercase)
      .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");

    RuleFor(x => x.UserName)
      .MaximumLength(256).WithMessage("Username must not exceed 256 characters.")
      .When(x => !string.IsNullOrEmpty(x.UserName));
  }

  private static bool HaveDigit(string password)
  {
    return password.Any(c => char.IsDigit(c));
  }

  private static bool HaveNonAlphanumericCharacter(string password)
  {
    return password.Any(c => !char.IsLetterOrDigit(c));
  }

  private static bool HaveLowercase(string password)
  {
    return password.Any(c => char.IsLower(c));
  }

  private static bool HaveUppercase(string password)
  {
    return password.Any(c => char.IsUpper(c));
  }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
  public UpdateUserValidator()
  {
    RuleFor(x => x.Email)
      .EmailAddress().WithMessage("Invalid email address.")
      .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
      .When(x => !string.IsNullOrEmpty(x.Email));

    RuleFor(x => x.UserName)
      .MaximumLength(256).WithMessage("Username must not exceed 256 characters.")
      .When(x => !string.IsNullOrEmpty(x.UserName));
  }
}

public class AssignRoleValidator : AbstractValidator<AssignRoleDto>
{
  public AssignRoleValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty().WithMessage("User ID is required.");

    RuleFor(x => x.Role)
      .NotEmpty().WithMessage("Role is required.")
      .Must(BeValidRole)
      .WithMessage($"Invalid role. Valid roles are: {Roles.Admin}, {Roles.HR}, {Roles.Recruiter}, {Roles.Interviewer}, {Roles.Reviewer}, {Roles.Candidate}, {Roles.Viewer}.");
  }

  private static bool BeValidRole(string role)
  {
    var validRoles = new[] { Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Candidate, Roles.Viewer };
    return validRoles.Contains(role);
  }
}

public class RemoveRoleValidator : AbstractValidator<RemoveRoleDto>
{
  public RemoveRoleValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty().WithMessage("User ID is required.");

    RuleFor(x => x.Role)
      .NotEmpty().WithMessage("Role is required.");
  }
}

public class UpdateUserRolesValidator : AbstractValidator<UpdateUserRolesDto>
{
  public UpdateUserRolesValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty().WithMessage("User ID is required.");

    RuleFor(x => x.Roles)
      .NotNull().WithMessage("Roles list is required.");

    RuleForEach(x => x.Roles)
      .Must(BeValidRole)
      .WithMessage("Invalid role. Valid roles are: Admin, HR, Recruiter, Interviewer, Reviewer, Candidate, Viewer.");
  }

  private static bool BeValidRole(string role)
  {
    var validRoles = new[] { Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Candidate, Roles.Viewer };
    return validRoles.Contains(role);
  }
}

public class UserQueryValidator : AbstractValidator<UserQueryDto>
{
  public UserQueryValidator()
  {
    RuleFor(x => x.Page)
      .GreaterThan(0).WithMessage("Page must be greater than 0.");

    RuleFor(x => x.PageSize)
      .GreaterThan(0).WithMessage("Page size must be greater than 0.")
      .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

    RuleFor(x => x.SortBy)
      .Must(BeValidSortField)
      .WithMessage("Invalid sort field. Valid fields are: Id, Email, UserName, CreatedAt, UpdatedAt.")
      .When(x => !string.IsNullOrEmpty(x.SortBy));

    RuleFor(x => x.Role)
      .Must(BeValidRole)
      .WithMessage("Invalid role filter.")
      .When(x => !string.IsNullOrEmpty(x.Role));
  }

  private static bool BeValidSortField(string? sortBy)
  {
    if (string.IsNullOrEmpty(sortBy)) return true;
    var validFields = new[] { "Id", "Email", "UserName", "CreatedAt", "UpdatedAt" };
    return validFields.Contains(sortBy);
  }

  private static bool BeValidRole(string? role)
  {
    if (string.IsNullOrEmpty(role)) return true;
    var validRoles = new[] { Roles.Admin, Roles.HR, Roles.Recruiter, Roles.Interviewer, Roles.Reviewer, Roles.Candidate, Roles.Viewer };
    return validRoles.Contains(role);
  }
}
