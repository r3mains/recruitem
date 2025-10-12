namespace Backend.Models;

public class Country
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;

  public ICollection<State> States { get; set; } = [];
}
