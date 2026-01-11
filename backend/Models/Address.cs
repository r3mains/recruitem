namespace backend.Models;

public class Address
{
  public Guid Id { get; set; }
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }

  public City? City { get; set; }
  public ICollection<Employee> Employees { get; set; } = [];
  public ICollection<Candidate> Candidates { get; set; } = [];
}
