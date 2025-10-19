namespace Backend.Dtos.Candidates;

public class UpdateCandidateDto
{
  public string? FullName { get; set; }
  public string? ContactNumber { get; set; }
  public string? ResumeUrl { get; set; }
  public UpdateAddressDto? Address { get; set; }
}

public class UpdateAddressDto
{
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }
}
