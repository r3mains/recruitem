namespace backend.DTOs.Location;

// Request DTOs
public record CreateStateDto(
  string StateName,
  Guid CountryId
);

public record UpdateStateDto(
  string StateName,
  Guid CountryId
);

// Response DTOs
public record StateDto(
  Guid Id,
  string StateName,
  Guid CountryId,
  string? CountryName = null
);

public record StateWithCitiesDto(
  Guid Id,
  string StateName,
  Guid CountryId,
  string CountryName,
  ICollection<CityDto> Cities
);
