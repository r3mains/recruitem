namespace backend.Models;

public class City
{
  public Guid Id { get; set; }
  public string CityName { get; set; } = string.Empty;
  public Guid StateId { get; set; }

  public State State { get; set; } = null!;
  public ICollection<Address> Addresses { get; set; } = [];
}
