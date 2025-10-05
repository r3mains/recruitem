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
  public DbSet<StatusType> StatusTypes => Set<StatusType>();
  public DbSet<JobType> JobTypes => Set<JobType>();

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
    });

    modelBuilder.Entity<Job>(e =>
    {
      e.ToTable("jobs");
      e.HasKey(x => x.Id);
      e.Property(x => x.Id).HasColumnName("id");
      e.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
      e.Property(x => x.Title).HasColumnName("title").IsRequired();
      e.Property(x => x.Description).HasColumnName("description").IsRequired();
      e.Property(x => x.JobTypeId).HasColumnName("job_type_id").IsRequired();
      e.Property(x => x.Location).HasColumnName("location").IsRequired();
      e.Property(x => x.SalaryMin).HasColumnName("salary_min");
      e.Property(x => x.SalaryMax).HasColumnName("salary_max");
      e.Property(x => x.PositionId).HasColumnName("position_id").IsRequired();
      e.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
      e.Property(x => x.ClosedReason).HasColumnName("closed_reason");
      e.Property(x => x.CreatedAt).HasColumnName("created_at");
      e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
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
  }
}
