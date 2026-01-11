using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class EventCandidate
  {
    public Guid Id { get; set; }

    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid StatusId { get; set; }
    public EventCandidateStatus Status { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
  }
}
