namespace backend.Models;

public class State
{
  public Guid Id { get; set; }
  public string StateName { get; set; } = string.Empty;
  public Guid CountryId { get; set; }

  public Country Country { get; set; } = null!;
  public ICollection<City> Cities { get; set; } = [];
}
