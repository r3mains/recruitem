namespace backend.Models;

public class Country
{
  public Guid Id { get; set; }
  public string CountryName { get; set; } = string.Empty;

  public ICollection<State> States { get; set; } = [];
}
