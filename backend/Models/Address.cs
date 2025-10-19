namespace Backend.Models;

public class Address
{
  public Guid Id { get; set; }
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Locality { get; set; }
  public string? Pincode { get; set; }
  public Guid? CityId { get; set; }

  public City? City { get; set; }
  public ICollection<Job> Jobs { get; set; } = [];
  public ICollection<Candidate> Candidates { get; set; } = [];
  public ICollection<Employee> Employees { get; set; } = [];

  public string? Street => AddressLine1;
  public string? State => City?.State?.Name;
  public string? ZipCode => Pincode;
}
