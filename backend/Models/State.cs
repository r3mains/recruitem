namespace Backend.Models;

public class State
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid CountryId { get; set; }

  public Country? Country { get; set; }
  public ICollection<City> Cities { get; set; } = [];
}
