using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Event
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public string? Location { get; set; }

    public DateTime Date { get; set; }

    public Guid CreatedBy { get; set; }
    public Employee CreatedByEmployee { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<EventCandidate> EventCandidates { get; set; } = new List<EventCandidate>();
  }
}
