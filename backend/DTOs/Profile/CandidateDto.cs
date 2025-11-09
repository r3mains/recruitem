using backend.DTOs.Location;

namespace backend.DTOs.Profile;

// Request DTOs
public record CreateCandidateDto(
  Guid UserId,
  string? FullName,
  string? ContactNumber,
  Guid? AddressId
);

public record UpdateCandidateDto(
  string? FullName,
  string? ContactNumber,
  Guid? AddressId
);

// Response DTOs
public record CandidateDto(
  Guid Id,
  Guid UserId,
  string? FullName,
  string? ContactNumber,
  Guid? AddressId,
  DateTime CreatedAt,
  DateTime UpdatedAt,
  bool IsDeleted,
  string? UserEmail = null,
  string? UserName = null
);

public record CandidateWithDetailsDto(
  Guid Id,
  Guid UserId,
  string? FullName,
  string? ContactNumber,
  DateTime CreatedAt,
  DateTime UpdatedAt,
  bool IsDeleted,
  string? UserEmail,
  string? UserName,
  AddressDto? Address
);

public record CandidateProfileDto(
  Guid Id,
  string? FullName,
  string? ContactNumber,
  AddressDto? Address
);
