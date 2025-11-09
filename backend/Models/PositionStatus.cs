namespace backend.Models;

public class PositionStatus
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Status { get; set; } = string.Empty;

  // Navigation properties
  public virtual ICollection<Position> Positions { get; set; } = [];
}
