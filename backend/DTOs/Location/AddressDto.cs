namespace backend.DTOs.Location;

// Request DTOs
public record CreateAddressDto(
  string? AddressLine1,
  string? AddressLine2,
  string? Locality,
  string? Pincode,
  Guid? CityId
);

public record UpdateAddressDto(
  string? AddressLine1,
  string? AddressLine2,
  string? Locality,
  string? Pincode,
  Guid? CityId
);

// Response DTOs
public record AddressDto(
  Guid Id,
  string? AddressLine1,
  string? AddressLine2,
  string? Locality,
  string? Pincode,
  Guid? CityId,
  string? CityName = null,
  string? StateName = null,
  string? CountryName = null
);

public record FullAddressDto(
  Guid Id,
  string? AddressLine1,
  string? AddressLine2,
  string? Locality,
  string? Pincode,
  CityDto? City
);
