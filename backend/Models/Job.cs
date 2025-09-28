using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace recruitem_backend.Models
{
    public class Job
    {
        public Guid Id { get; set; }

        [Required]
        public Guid RecruiterId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string Location { get; set; } = "";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SalaryMin { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SalaryMax { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("RecruiterId")]
        public User Recruiter { get; set; } = null!;
    }
}
