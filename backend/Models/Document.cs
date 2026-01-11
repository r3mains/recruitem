using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class Document
  {
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;

    public string Url { get; set; } = string.Empty;

    public string? OriginalFileName { get; set; }
    public string? MimeType { get; set; }
    public long? SizeBytes { get; set; }
    public DateTime? UploadedAt { get; set; }

    public Guid? UploadedBy { get; set; }
    public User? UploadedByUser { get; set; }

    public ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = new List<ApplicationDocument>();
    public ICollection<Verification> Verifications { get; set; } = new List<Verification>();
  }
}
