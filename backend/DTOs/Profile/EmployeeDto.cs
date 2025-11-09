using backend.DTOs.Location;

namespace backend.DTOs.Profile;

// Request DTOs
public record CreateEmployeeDto(
  Guid UserId,
  string? FullName,
  Guid? BranchAddressId,
  DateOnly? JoiningDate,
  string? OfferLetterUrl
);

public record UpdateEmployeeDto(
  string? FullName,
  Guid? BranchAddressId,
  DateOnly? JoiningDate,
  string? OfferLetterUrl
);

// Response DTOs
public record EmployeeDto(
  Guid Id,
  Guid UserId,
  string? FullName,
  Guid? BranchAddressId,
  DateOnly? JoiningDate,
  string? OfferLetterUrl,
  DateTime CreatedAt,
  DateTime UpdatedAt,
  bool IsDeleted,
  string? UserEmail = null,
  string? UserName = null
);

public record EmployeeWithDetailsDto(
  Guid Id,
  Guid UserId,
  string? FullName,
  DateOnly? JoiningDate,
  string? OfferLetterUrl,
  DateTime CreatedAt,
  DateTime UpdatedAt,
  bool IsDeleted,
  string? UserEmail,
  string? UserName,
  AddressDto? BranchAddress
);

public record EmployeeProfileDto(
  Guid Id,
  string? FullName,
  DateOnly? JoiningDate,
  string? OfferLetterUrl,
  AddressDto? BranchAddress
);
