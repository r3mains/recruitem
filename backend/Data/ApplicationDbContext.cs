using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using backend.Models;
using backend.Consts;

namespace backend.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
  public DbSet<Country> Countries { get; set; }
  public DbSet<State> States { get; set; }
  public DbSet<City> Cities { get; set; }
  public DbSet<Address> Addresses { get; set; }
  public DbSet<Employee> Employees { get; set; }
  public DbSet<Candidate> Candidates { get; set; }
  public DbSet<Models.PositionStatus> PositionStatuses { get; set; }
  public DbSet<Models.JobStatus> JobStatuses { get; set; }
  public DbSet<Models.JobType> JobTypes { get; set; }
  public DbSet<Skill> Skills { get; set; }
  public DbSet<Qualification> Qualifications { get; set; }
  public DbSet<Position> Positions { get; set; }
  public DbSet<PositionSkill> PositionSkills { get; set; }
  public DbSet<Job> Jobs { get; set; }
  public DbSet<JobSkill> JobSkills { get; set; }
  public DbSet<JobQualification> JobQualifications { get; set; }
  public DbSet<Models.ApplicationStatus> ApplicationStatuses { get; set; }
  public DbSet<Models.InterviewStatus> InterviewStatuses { get; set; }
  public DbSet<Models.ScheduleStatus> ScheduleStatuses { get; set; }
  public DbSet<Models.EventCandidateStatus> EventCandidateStatuses { get; set; }
  public DbSet<Models.VerificationStatus> VerificationStatuses { get; set; }
  public DbSet<Models.DocumentType> DocumentTypes { get; set; }
  public DbSet<Document> Documents { get; set; }
  public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
  public DbSet<JobApplication> JobApplications { get; set; }
  public DbSet<Comment> Comments { get; set; }
  public DbSet<OnlineTest> OnlineTests { get; set; }
  public DbSet<ApplicationStatusHistory> ApplicationStatusHistory { get; set; }
  public DbSet<Verification> Verifications { get; set; }
  public DbSet<CandidateSkill> CandidateSkills { get; set; }
  public DbSet<CandidateQualification> CandidateQualifications { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    ConfigureEntities(builder);

    SeedStatuses(builder);
  }

  private void ConfigureEntities(ModelBuilder builder)
  {
    builder.Entity<Position>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.ClosedReason).HasMaxLength(500);
      entity.Property(e => e.NumberOfInterviews).IsRequired();

      entity.HasOne(e => e.Status)
            .WithMany(s => s.Positions)
            .HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.Reviewer)
            .WithMany(r => r.ReviewedPositions)
            .HasForeignKey(e => e.ReviewerId)
            .OnDelete(DeleteBehavior.SetNull);
    });

    builder.Entity<PositionSkill>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.PositionId, e.SkillId }).IsUnique();

      entity.HasOne(e => e.Position)
            .WithMany(p => p.PositionSkills)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Skill)
            .WithMany(s => s.PositionSkills)
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<Job>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
      entity.Property(e => e.SalaryMin).HasColumnType("decimal(12,2)");
      entity.Property(e => e.SalaryMax).HasColumnType("decimal(12,2)");
      entity.Property(e => e.ClosedReason).HasMaxLength(500);

      entity.HasOne(e => e.Recruiter)
            .WithMany(r => r.RecruitedJobs)
            .HasForeignKey(e => e.RecruiterId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.JobType)
            .WithMany(jt => jt.Jobs)
            .HasForeignKey(e => e.JobTypeId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.Address)
            .WithMany()
            .HasForeignKey(e => e.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.Position)
            .WithMany(p => p.Jobs)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.Status)
            .WithMany(s => s.Jobs)
            .HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.ToTable(t => t.HasCheckConstraint("CK_Job_Salary", "\"SalaryMin\" IS NULL OR \"SalaryMax\" IS NULL OR \"SalaryMin\" <= \"SalaryMax\""));
    });

    builder.Entity<JobSkill>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.HasOne(e => e.Job)
            .WithMany(j => j.JobSkills)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Skill)
            .WithMany(s => s.JobSkills)
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<JobQualification>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.HasOne(e => e.Job)
            .WithMany(j => j.JobQualifications)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Qualification)
            .WithMany(q => q.JobQualifications)
            .HasForeignKey(e => e.QualificationId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<Models.PositionStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.JobStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.JobType>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.ApplicationStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.InterviewStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.ScheduleStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.EventCandidateStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
      entity.HasIndex(e => e.Status).IsUnique();
    });

    builder.Entity<Models.VerificationStatus>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    });

    builder.Entity<Models.DocumentType>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
    });

    builder.Entity<Skill>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.SkillName).IsRequired().HasMaxLength(100);
    });

    builder.Entity<Qualification>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.QualificationName).IsRequired().HasMaxLength(100);
    });

    builder.Entity<Document>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Url).IsRequired();
      entity.HasIndex(e => e.Url).IsUnique();

      entity.HasOne(e => e.Candidate)
            .WithMany(c => c.Documents)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.DocumentType)
            .WithMany(dt => dt.Documents)
            .HasForeignKey(e => e.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.UploadedByUser)
            .WithMany(u => u.UploadedDocuments)
            .HasForeignKey(e => e.UploadedBy)
            .OnDelete(DeleteBehavior.SetNull);
    });

    builder.Entity<JobApplication>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Score).HasColumnType("decimal(5,2)");

      entity.HasOne(e => e.Job)
            .WithMany(j => j.JobApplications)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Candidate)
            .WithMany(c => c.JobApplications)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Status)
            .WithMany(s => s.JobApplications)
            .HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.CreatedByUser)
            .WithMany(u => u.CreatedJobApplications)
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

      entity.HasOne(e => e.UpdatedByUser)
            .WithMany(u => u.UpdatedJobApplications)
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    });

    builder.Entity<ApplicationDocument>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.HasOne(e => e.JobApplication)
            .WithMany(ja => ja.ApplicationDocuments)
            .HasForeignKey(e => e.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Document)
            .WithMany(d => d.ApplicationDocuments)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<Comment>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CommentText).IsRequired();

      entity.HasOne(e => e.JobApplication)
            .WithMany(ja => ja.Comments)
            .HasForeignKey(e => e.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Commenter)
            .WithMany(emp => emp.Comments)
            .HasForeignKey(e => e.CommenterId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<OnlineTest>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Score).HasColumnType("decimal(5,2)");

      entity.HasOne(e => e.JobApplication)
            .WithMany(ja => ja.OnlineTests)
            .HasForeignKey(e => e.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<ApplicationStatusHistory>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.ChangedAt).IsRequired();

      entity.HasOne(e => e.JobApplication)
            .WithMany(ja => ja.StatusHistory)
            .HasForeignKey(e => e.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Status)
            .WithMany(s => s.StatusHistory)
            .HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.ChangedByUser)
            .WithMany(u => u.StatusChanges)
            .HasForeignKey(e => e.ChangedBy)
            .OnDelete(DeleteBehavior.SetNull);
    });

    builder.Entity<Verification>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.HasOne(e => e.Candidate)
            .WithMany(c => c.Verifications)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Document)
            .WithMany(d => d.Verifications)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Status)
            .WithMany(vs => vs.Verifications)
            .HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.VerifiedByEmployee)
            .WithMany(emp => emp.Verifications)
            .HasForeignKey(e => e.VerifiedBy)
            .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<CandidateSkill>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.HasOne(e => e.Candidate)
            .WithMany(c => c.CandidateSkills)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Skill)
            .WithMany(s => s.CandidateSkills)
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasIndex(e => new { e.CandidateId, e.SkillId }).IsUnique();
    });

    builder.Entity<CandidateQualification>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CompletedOn).HasColumnType("date");
      entity.Property(e => e.Grade).HasColumnType("decimal(5,2)");

      entity.HasOne(e => e.Candidate)
            .WithMany(c => c.CandidateQualifications)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Qualification)
            .WithMany(q => q.CandidateQualifications)
            .HasForeignKey(e => e.QualificationId)
            .OnDelete(DeleteBehavior.Cascade);

      entity.HasIndex(e => new { e.CandidateId, e.QualificationId }).IsUnique();
    });
  }

  private void SeedStatuses(ModelBuilder builder)
  {
    builder.Entity<Models.PositionStatus>().HasData(
      new Models.PositionStatus { Id = new Guid("10000000-0000-0000-0000-000000000001"), Status = Consts.PositionStatus.Open },
      new Models.PositionStatus { Id = new Guid("10000000-0000-0000-0000-000000000002"), Status = Consts.PositionStatus.OnHold },
      new Models.PositionStatus { Id = new Guid("10000000-0000-0000-0000-000000000003"), Status = Consts.PositionStatus.Closed }
    );

    builder.Entity<Models.JobStatus>().HasData(
      new Models.JobStatus { Id = new Guid("20000000-0000-0000-0000-000000000001"), Status = Consts.JobStatus.Open },
      new Models.JobStatus { Id = new Guid("20000000-0000-0000-0000-000000000002"), Status = Consts.JobStatus.OnHold },
      new Models.JobStatus { Id = new Guid("20000000-0000-0000-0000-000000000003"), Status = Consts.JobStatus.Closed }
    );

    builder.Entity<Models.JobType>().HasData(
      new Models.JobType { Id = new Guid("30000000-0000-0000-0000-000000000001"), Type = Consts.JobType.Internship },
      new Models.JobType { Id = new Guid("30000000-0000-0000-0000-000000000002"), Type = Consts.JobType.FullTime }
    );

    builder.Entity<Models.ApplicationStatus>().HasData(
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000001"), Status = Consts.ApplicationStatus.Applied },
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000002"), Status = Consts.ApplicationStatus.Screening },
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000003"), Status = Consts.ApplicationStatus.Shortlisted },
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000004"), Status = Consts.ApplicationStatus.Interview },
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000005"), Status = Consts.ApplicationStatus.Selected },
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000006"), Status = Consts.ApplicationStatus.Rejected },
      new Models.ApplicationStatus { Id = new Guid("40000000-0000-0000-0000-000000000007"), Status = Consts.ApplicationStatus.OnHold }
    );

    builder.Entity<Models.InterviewStatus>().HasData(
      new Models.InterviewStatus { Id = new Guid("50000000-0000-0000-0000-000000000001"), Status = Consts.InterviewStatus.Planned },
      new Models.InterviewStatus { Id = new Guid("50000000-0000-0000-0000-000000000002"), Status = Consts.InterviewStatus.InProgress },
      new Models.InterviewStatus { Id = new Guid("50000000-0000-0000-0000-000000000003"), Status = Consts.InterviewStatus.Completed },
      new Models.InterviewStatus { Id = new Guid("50000000-0000-0000-0000-000000000004"), Status = Consts.InterviewStatus.Cancelled }
    );

    builder.Entity<Models.ScheduleStatus>().HasData(
      new Models.ScheduleStatus { Id = new Guid("60000000-0000-0000-0000-000000000001"), Status = Consts.ScheduleStatus.Scheduled },
      new Models.ScheduleStatus { Id = new Guid("60000000-0000-0000-0000-000000000002"), Status = Consts.ScheduleStatus.Rescheduled },
      new Models.ScheduleStatus { Id = new Guid("60000000-0000-0000-0000-000000000003"), Status = Consts.ScheduleStatus.Completed },
      new Models.ScheduleStatus { Id = new Guid("60000000-0000-0000-0000-000000000004"), Status = Consts.ScheduleStatus.Cancelled }
    );

    builder.Entity<Models.EventCandidateStatus>().HasData(
      new Models.EventCandidateStatus { Id = new Guid("70000000-0000-0000-0000-000000000001"), Status = Consts.EventCandidateStatus.Registered },
      new Models.EventCandidateStatus { Id = new Guid("70000000-0000-0000-0000-000000000002"), Status = Consts.EventCandidateStatus.CheckedIn },
      new Models.EventCandidateStatus { Id = new Guid("70000000-0000-0000-0000-000000000003"), Status = Consts.EventCandidateStatus.Interviewed },
      new Models.EventCandidateStatus { Id = new Guid("70000000-0000-0000-0000-000000000004"), Status = Consts.EventCandidateStatus.Selected },
      new Models.EventCandidateStatus { Id = new Guid("70000000-0000-0000-0000-000000000005"), Status = Consts.EventCandidateStatus.Rejected }
    );

    builder.Entity<Models.VerificationStatus>().HasData(
      new Models.VerificationStatus { Id = new Guid("80000000-0000-0000-0000-000000000001"), Status = Consts.VerificationStatus.Pending },
      new Models.VerificationStatus { Id = new Guid("80000000-0000-0000-0000-000000000002"), Status = Consts.VerificationStatus.InProgress },
      new Models.VerificationStatus { Id = new Guid("80000000-0000-0000-0000-000000000003"), Status = Consts.VerificationStatus.Verified },
      new Models.VerificationStatus { Id = new Guid("80000000-0000-0000-0000-000000000004"), Status = Consts.VerificationStatus.Rejected }
    );

    builder.Entity<Models.DocumentType>().HasData(
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000001"), Type = Consts.DocumentType.Resume },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000002"), Type = Consts.DocumentType.CoverLetter },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000003"), Type = Consts.DocumentType.Transcript },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000004"), Type = Consts.DocumentType.Certificate },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000005"), Type = Consts.DocumentType.IdentityProof },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000006"), Type = Consts.DocumentType.AddressProof },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000007"), Type = Consts.DocumentType.Portfolio },
      new Models.DocumentType { Id = new Guid("90000000-0000-0000-0000-000000000008"), Type = Consts.DocumentType.References }
    );
  }
}
