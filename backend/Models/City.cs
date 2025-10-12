namespace Backend.Models;

public class City
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid StateId { get; set; }

  public State? State { get; set; }
  public ICollection<Address> Addresses { get; set; } = [];
}
