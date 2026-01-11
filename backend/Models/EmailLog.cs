using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class EmailLog
  {
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public string ToEmail { get; set; } = null!;

    public string? Subject { get; set; }

    public string? TemplateName { get; set; }

    public string? Body { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = "Sent";

    public string? ErrorMessage { get; set; }
  }
}
