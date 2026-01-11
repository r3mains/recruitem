using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
  public class JobApplication
  {
    public Guid Id { get; set; }

    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid StatusId { get; set; }
    public ApplicationStatus Status { get; set; } = null!;

    public decimal? Score { get; set; }
    public int? NumberOfInterviewRounds { get; set; }

    public DateTime? AppliedAt { get; set; }
    public DateTime? LastUpdated { get; set; }

    public Guid? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    public Guid? UpdatedBy { get; set; }
    public User? UpdatedByUser { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = new List<ApplicationDocument>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
  }
}
