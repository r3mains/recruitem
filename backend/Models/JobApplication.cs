using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
  public class JobApplication
  {
    public Guid Id { get; set; }

    [Required]
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    [Required]
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    [Required]
    public Guid StatusId { get; set; }
    public ApplicationStatus Status { get; set; } = null!;

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Score { get; set; }

    public DateTime? AppliedAt { get; set; }
    public DateTime? LastUpdated { get; set; }

    public Guid? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    public Guid? UpdatedBy { get; set; }
    public User? UpdatedByUser { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = new List<ApplicationDocument>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<OnlineTest> OnlineTests { get; set; } = new List<OnlineTest>();
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
  }
}
