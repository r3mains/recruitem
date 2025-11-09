namespace backend.DTOs.Location;

// Request DTOs
public record CreateCityDto(
  string CityName,
  Guid StateId
);

public record UpdateCityDto(
  string CityName,
  Guid StateId
);

// Response DTOs
public record CityDto(
  Guid Id,
  string CityName,
  Guid StateId,
  string? StateName = null,
  string? CountryName = null
);

public record CityWithAddressesDto(
  Guid Id,
  string CityName,
  Guid StateId,
  string StateName,
  string CountryName,
  ICollection<AddressDto> Addresses
);
