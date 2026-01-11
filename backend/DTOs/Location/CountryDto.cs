namespace backend.DTOs.Location;

// Request DTOs
public record CreateCountryDto(
  string CountryName
);

public record UpdateCountryDto(
  string CountryName
);

// Response DTOs
public record CountryDto(
  Guid Id,
  string CountryName
);

public record CountryWithStatesDto(
  Guid Id,
  string CountryName,
  ICollection<StateDto> States
);
