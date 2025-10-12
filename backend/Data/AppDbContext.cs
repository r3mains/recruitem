using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<User> Users => Set<User>();
  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Candidate> Candidates => Set<Candidate>();
  public DbSet<Employee> Employees => Set<Employee>();
  public DbSet<Job> Jobs => Set<Job>();
  public DbSet<JobApplication> JobApplications => Set<JobApplication>();
  public DbSet<StatusType> StatusTypes => Set<StatusType>();
  public DbSet<JobType> JobTypes => Set<JobType>();
  public DbSet<Position> Positions => Set<Position>();
  public DbSet<Address> Addresses => Set<Address>();
  public DbSet<City> Cities => Set<City>();
  public DbSet<State> States => Set<State>();
  public DbSet<Country> Countries => Set<Country>();
  public DbSet<Skill> Skills => Set<Skill>();
  public DbSet<JobSkill> JobSkills => Set<JobSkill>();
  public DbSet<PositionSkill> PositionSkills => Set<PositionSkill>();
  public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Role>(e =>
    {
      e.ToTable("roles");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Name).HasColumnName("role");
    });

    modelBuilder.Entity<User>(e =>
    {
      e.ToTable("users");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Email).HasColumnName("email").IsRequired();
      e.Property(x => x.Password).HasColumnName("password").IsRequired();
      e.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
      e.Property(x => x.CreatedAt).HasColumnName("created_at");
      e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

      e.HasOne(x => x.Role)
        .WithMany(r => r.Users)
        .HasForeignKey(x => x.RoleId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Candidate>(e =>
    {
      e.ToTable("candidates");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
      e.Property(x => x.FullName).HasColumnName("full_name");
      e.Property(x => x.ContactNumber).HasColumnName("contact_number");
      e.Property(x => x.ResumeUrl).HasColumnName("resume_url");
      e.Property(x => x.AddressId).HasColumnName("address");

      e.HasOne(x => x.User)
        .WithOne(u => u.Candidate)
        .HasForeignKey<Candidate>(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Address)
        .WithMany(addr => addr.Candidates)
        .HasForeignKey(x => x.AddressId)
        .OnDelete(DeleteBehavior.SetNull);
    });

    modelBuilder.Entity<Employee>(e =>
    {
      e.ToTable("employees");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
      e.Property(x => x.FullName).HasColumnName("full_name");
      e.Property(x => x.BranchAddressId).HasColumnName("branch_address");
      e.Property(x => x.JoiningDate).HasColumnName("joining_date");
      e.Property(x => x.OfferLetterUrl).HasColumnName("offer_letter_url");

      e.HasOne(x => x.User)
        .WithOne(u => u.Employee)
        .HasForeignKey<Employee>(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.BranchAddress)
        .WithMany(addr => addr.Employees)
        .HasForeignKey(x => x.BranchAddressId)
        .OnDelete(DeleteBehavior.SetNull);
    });

    modelBuilder.Entity<Job>(e =>
    {
      e.ToTable("jobs");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.RecruiterId).HasColumnName("recruiter_id");
      e.Property(x => x.Title).HasColumnName("title").IsRequired();
      e.Property(x => x.Description).HasColumnName("description").IsRequired();
      e.Property(x => x.JobTypeId).HasColumnName("job_type_id").IsRequired();
      e.Property(x => x.LocationId).HasColumnName("location").IsRequired();
      e.Property(x => x.SalaryMin).HasColumnName("salary_min");
      e.Property(x => x.SalaryMax).HasColumnName("salary_max");
      e.Property(x => x.PositionId).HasColumnName("position_id").IsRequired();
      e.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
      e.Property(x => x.ClosedReason).HasColumnName("closed_reason");
      e.Property(x => x.CreatedAt).HasColumnName("created_at");
      e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

      e.HasOne(x => x.Recruiter)
        .WithMany(emp => emp.RecruitedJobs)
        .HasForeignKey(x => x.RecruiterId)
        .OnDelete(DeleteBehavior.SetNull);

      e.HasOne(x => x.Location)
        .WithMany(addr => addr.Jobs)
        .HasForeignKey(x => x.LocationId)
        .OnDelete(DeleteBehavior.Restrict);

      e.HasOne(x => x.Position)
        .WithMany(pos => pos.Jobs)
        .HasForeignKey(x => x.PositionId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<StatusType>(e =>
    {
      e.ToTable("status_types");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Context).HasColumnName("context").IsRequired();
      e.Property(x => x.Status).HasColumnName("status").IsRequired();
    });

    modelBuilder.Entity<JobType>(e =>
    {
      e.ToTable("job_types");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Type).HasColumnName("type");
    });

    modelBuilder.Entity<JobApplication>(e =>
    {
      e.ToTable("job_applications");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.JobId).HasColumnName("job_id").IsRequired();
      e.Property(x => x.CandidateId).HasColumnName("candidate_id").IsRequired();
      e.Property(x => x.AppliedAt).HasColumnName("applied_at").IsRequired();
      e.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
      e.Property(x => x.CoverLetter).HasColumnName("cover_letter");
      e.Property(x => x.Notes).HasColumnName("notes");
      e.Property(x => x.ReviewedAt).HasColumnName("reviewed_at");
      e.Property(x => x.ReviewedBy).HasColumnName("reviewed_by");

      e.HasOne(x => x.Job)
        .WithMany(j => j.JobApplications)
        .HasForeignKey(x => x.JobId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Candidate)
        .WithMany(c => c.JobApplications)
        .HasForeignKey(x => x.CandidateId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Position>(e =>
    {
      e.ToTable("positions");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Title).HasColumnName("title");
      e.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
      e.Property(x => x.ClosedReason).HasColumnName("closed_reason");
      e.Property(x => x.NumberOfInterviews).HasColumnName("number_of_interviews").IsRequired();
      e.Property(x => x.ReviewerId).HasColumnName("reviewer_id");

      e.HasOne(x => x.Reviewer)
        .WithMany(emp => emp.ReviewedPositions)
        .HasForeignKey(x => x.ReviewerId)
        .OnDelete(DeleteBehavior.SetNull);
    });

    modelBuilder.Entity<Address>(e =>
    {
      e.ToTable("addresses");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.AddressLine1).HasColumnName("address_line_1");
      e.Property(x => x.AddressLine2).HasColumnName("address_line_2");
      e.Property(x => x.Locality).HasColumnName("locality");
      e.Property(x => x.Pincode).HasColumnName("pincode");
      e.Property(x => x.CityId).HasColumnName("city_id");

      e.HasOne(x => x.City)
        .WithMany(c => c.Addresses)
        .HasForeignKey(x => x.CityId)
        .OnDelete(DeleteBehavior.SetNull);
    });

    modelBuilder.Entity<City>(e =>
    {
      e.ToTable("cities");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Name).HasColumnName("city").IsRequired();
      e.Property(x => x.StateId).HasColumnName("state_id").IsRequired();

      e.HasOne(x => x.State)
        .WithMany(s => s.Cities)
        .HasForeignKey(x => x.StateId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<State>(e =>
    {
      e.ToTable("states");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Name).HasColumnName("state").IsRequired();
      e.Property(x => x.CountryId).HasColumnName("country_id").IsRequired();

      e.HasOne(x => x.Country)
        .WithMany(c => c.States)
        .HasForeignKey(x => x.CountryId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Country>(e =>
    {
      e.ToTable("countries");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Name).HasColumnName("country").IsRequired();
    });

    modelBuilder.Entity<Skill>(e =>
    {
      e.ToTable("skills");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.Name).HasColumnName("skill");
    });

    modelBuilder.Entity<JobSkill>(e =>
    {
      e.ToTable("job_skills");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.JobId).HasColumnName("job_id").IsRequired();
      e.Property(x => x.SkillId).HasColumnName("skill_id").IsRequired();
      e.Property(x => x.Required).HasColumnName("required").IsRequired();

      e.HasOne(x => x.Job)
        .WithMany(j => j.JobSkills)
        .HasForeignKey(x => x.JobId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Skill)
        .WithMany(s => s.JobSkills)
        .HasForeignKey(x => x.SkillId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<PositionSkill>(e =>
    {
      e.ToTable("position_skills");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.PositionId).HasColumnName("position_id").IsRequired();
      e.Property(x => x.SkillId).HasColumnName("skill_id").IsRequired();

      e.HasOne(x => x.Position)
        .WithMany(p => p.PositionSkills)
        .HasForeignKey(x => x.PositionId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Skill)
        .WithMany(s => s.PositionSkills)
        .HasForeignKey(x => x.SkillId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<CandidateSkill>(e =>
    {
      e.ToTable("candidate_skills");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.CandidateId).HasColumnName("candidate_id").IsRequired();
      e.Property(x => x.SkillId).HasColumnName("skill_id").IsRequired();
      e.Property(x => x.YearOfExperience).HasColumnName("year_of_experience");

      e.HasOne(x => x.Candidate)
        .WithMany(c => c.CandidateSkills)
        .HasForeignKey(x => x.CandidateId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Skill)
        .WithMany(s => s.CandidateSkills)
        .HasForeignKey(x => x.SkillId)
        .OnDelete(DeleteBehavior.Cascade);
    });
  }
}
