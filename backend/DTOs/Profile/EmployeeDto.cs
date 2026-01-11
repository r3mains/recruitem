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

public class EmployeeWithDetailsDto
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? FullName { get; set; }
  public DateOnly? JoiningDate { get; set; }
  public string? OfferLetterUrl { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public bool IsDeleted { get; set; }
  public string? UserEmail { get; set; }
  public string? UserName { get; set; }
  public AddressDto? BranchAddress { get; set; }
}

public record EmployeeProfileDto(
  Guid Id,
  string? FullName,
  DateOnly? JoiningDate,
  string? OfferLetterUrl,
  AddressDto? BranchAddress
);
