namespace Backend.Dtos.Addresses;

public class AddressDto
{
  public Guid Id { get; set; }
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }
  public string? CityName { get; set; }
  public string? StateName { get; set; }
  public string? CountryName { get; set; }
}

public class AddressCreateDto
{
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }
}

public class AddressUpdateDto
{
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }
}
