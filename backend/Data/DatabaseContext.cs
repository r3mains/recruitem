using Microsoft.EntityFrameworkCore;
using recruitem_backend.Models;

namespace recruitem_backend.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.CreatedAt).HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                entity.Property(e => e.UpdatedAt).HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoleName).IsRequired();
            });

            builder.Entity<Job>(entity =>
            {
                entity.ToTable("jobs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.CreatedAt).HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                entity.Property(e => e.UpdatedAt).HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                entity.HasOne(e => e.Recruiter)
                      .WithMany(u => u.Jobs)
                      .HasForeignKey(e => e.RecruiterId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(builder);
        }

        private static void SeedData(ModelBuilder builder)
        {
            var adminRoleId = Guid.NewGuid();
            var recruiterRoleId = Guid.NewGuid();
            var hrRoleId = Guid.NewGuid();
            var interviewerRoleId = Guid.NewGuid();
            var reviewerRoleId = Guid.NewGuid();
            var candidateRoleId = Guid.NewGuid();
            var viewerRoleId = Guid.NewGuid();

            builder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, RoleName = "Admin" },
                new Role { Id = recruiterRoleId, RoleName = "Recruiter" },
                new Role { Id = hrRoleId, RoleName = "HR" },
                new Role { Id = interviewerRoleId, RoleName = "Interviewer" },
                new Role { Id = reviewerRoleId, RoleName = "Reviewer" },
                new Role { Id = candidateRoleId, RoleName = "Candidate" },
                new Role { Id = viewerRoleId, RoleName = "Viewer" }
            );
        }
    }
}
