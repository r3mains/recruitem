using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Document
  {
    public Guid Id { get; set; }

    [Required]
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    [Required]
    public Guid DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;

    [Required]
    public string Url { get; set; } = string.Empty;

    public string? OriginalFileName { get; set; }
    public string? MimeType { get; set; }
    public long? SizeBytes { get; set; }
    public DateTime? UploadedAt { get; set; }

    public Guid? UploadedBy { get; set; }
    public User? UploadedByUser { get; set; }

    // Navigation properties
    public ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = new List<ApplicationDocument>();
    public ICollection<Verification> Verifications { get; set; } = new List<Verification>();
  }
}
