using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs.Job;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
public class JobRepository(ApplicationDbContext context) : IJobRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Job> CreateJobAsync(Job job)
    {
      _context.Jobs.Add(job);
      await _context.SaveChangesAsync();
      return job;
    }

    public async Task<Job?> GetJobByIdAsync(Guid id)
    {
      return await _context.Jobs
          .Include(j => j.Recruiter)
          .Include(j => j.JobType)
          .Include(j => j.Address)
              .ThenInclude(a => a!.City)
                  .ThenInclude(c => c!.State)
                      .ThenInclude(s => s!.Country)
          .Include(j => j.Position)
          .Include(j => j.Status)
          .Include(j => j.JobSkills)
              .ThenInclude(js => js.Skill)
          .Include(j => j.JobQualifications)
              .ThenInclude(jq => jq.Qualification)
          .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
    }

    public async Task<JobResponseDto?> GetJobDetailsByIdAsync(Guid id)
    {
      return await _context.Jobs
          .Where(j => j.Id == id && !j.IsDeleted)
          .Select(j => new JobResponseDto
          {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            ClosedReason = j.ClosedReason,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt,
            Recruiter = new JobRecruiterDto
            {
              Id = j.Recruiter.Id,
              FullName = j.Recruiter.FullName,
              Email = j.Recruiter.User.Email
            },
            JobType = new JobTypeDto
            {
              Id = j.JobType.Id,
              Type = j.JobType.Type
            },
            Address = new JobAddressDto
            {
              Id = j.Address.Id,
              AddressLine1 = j.Address.AddressLine1,
              AddressLine2 = j.Address.AddressLine2,
              Locality = j.Address.Locality,
              Pincode = j.Address.Pincode,
              CityName = j.Address.City != null ? j.Address.City.CityName : null,
              StateName = j.Address.City != null && j.Address.City.State != null ? j.Address.City.State.StateName : null,
              CountryName = j.Address.City != null && j.Address.City.State != null && j.Address.City.State.Country != null ? j.Address.City.State.Country.CountryName : null
            },
            Position = new JobPositionDto
            {
              Id = j.Position.Id,
              Title = j.Position.Title,
              Status = j.Position.Status.Status
            },
            Status = new JobStatusDto
            {
              Id = j.Status.Id,
              Status = j.Status.Status
            },
            Skills = j.JobSkills.Select(js => new JobSkillDto
            {
              Id = js.Skill.Id,
              SkillName = js.Skill.SkillName,
              Required = js.Required
            }).ToList(),
            Qualifications = j.JobQualifications.Select(jq => new JobQualificationResponseDto
            {
              Id = jq.Qualification.Id,
              QualificationName = jq.Qualification.QualificationName,
              MinRequired = jq.MinRequired
            }).ToList(),
            ApplicationCount = j.JobApplications.Count(ja => !ja.IsDeleted)
          })
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<JobListDto>> GetJobsAsync(int page = 1, int pageSize = 10, string? search = null, Guid? statusId = null, Guid? jobTypeId = null)
    {
      var query = _context.Jobs
          .Where(j => !j.IsDeleted);

      if (!string.IsNullOrEmpty(search))
      {
        query = query.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
      }

      if (statusId.HasValue)
      {
        query = query.Where(j => j.StatusId == statusId.Value);
      }

      if (jobTypeId.HasValue)
      {
        query = query.Where(j => j.JobTypeId == jobTypeId.Value);
      }

      return await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(j => new JobListDto
          {
            Id = j.Id,
            Title = j.Title,
            JobType = j.JobType.Type,
            Status = j.Status.Status,
            Location = j.Address.City != null ? $"{j.Address.City.CityName}, {j.Address.City.State.StateName}" : "N/A",
            RecruiterName = j.Recruiter.FullName ?? "N/A",
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            CreatedAt = j.CreatedAt,
            ApplicationCount = j.JobApplications.Count(ja => !ja.IsDeleted)
          })
          .OrderByDescending(j => j.CreatedAt)
          .ToListAsync();
    }

    public async Task<Job> UpdateJobAsync(Job job)
    {
      _context.Jobs.Update(job);
      await _context.SaveChangesAsync();
      return job;
    }

    public async Task DeleteJobAsync(Guid id)
    {
      var job = await _context.Jobs.FindAsync(id);
      if (job != null)
      {
        job.IsDeleted = true;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
      }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
      return await _context.Jobs.AnyAsync(j => j.Id == id && !j.IsDeleted);
    }

    public async Task<IEnumerable<JobListDto>> GetJobsByRecruiterAsync(Guid recruiterId, int page = 1, int pageSize = 10)
    {
      return await _context.Jobs
          .Where(j => j.RecruiterId == recruiterId && !j.IsDeleted)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(j => new JobListDto
          {
            Id = j.Id,
            Title = j.Title,
            JobType = j.JobType.Type,
            Status = j.Status.Status,
            Location = j.Address.City != null ? $"{j.Address.City.CityName}, {j.Address.City.State.StateName}" : "N/A",
            RecruiterName = j.Recruiter.FullName ?? "N/A",
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            CreatedAt = j.CreatedAt,
            ApplicationCount = j.JobApplications.Count(ja => !ja.IsDeleted)
          })
          .OrderByDescending(j => j.CreatedAt)
          .ToListAsync();
    }

    public async Task<IEnumerable<JobListDto>> GetJobsByPositionAsync(Guid positionId)
    {
      return await _context.Jobs
          .Where(j => j.PositionId == positionId && !j.IsDeleted)
          .Select(j => new JobListDto
          {
            Id = j.Id,
            Title = j.Title,
            JobType = j.JobType.Type,
            Status = j.Status.Status,
            Location = j.Address.City != null ? $"{j.Address.City.CityName}, {j.Address.City.State.StateName}" : "N/A",
            RecruiterName = j.Recruiter.FullName ?? "N/A",
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            CreatedAt = j.CreatedAt,
            ApplicationCount = j.JobApplications.Count(ja => !ja.IsDeleted)
          })
          .OrderByDescending(j => j.CreatedAt)
          .ToListAsync();
    }

    public async Task<int> GetJobCountAsync(Guid? statusId = null, Guid? recruiterId = null)
    {
      var query = _context.Jobs.Where(j => !j.IsDeleted);

      if (statusId.HasValue)
      {
        query = query.Where(j => j.StatusId == statusId.Value);
      }

      if (recruiterId.HasValue)
      {
        query = query.Where(j => j.RecruiterId == recruiterId.Value);
      }

      return await query.CountAsync();
    }

    public async Task<bool> CanUpdateJobAsync(Guid jobId, Guid userId)
    {
      var job = await _context.Jobs
          .Include(j => j.Recruiter)
          .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted);

      if (job == null) return false;

      return job.Recruiter.UserId == userId;
    }

    public async Task<bool> CanDeleteJobAsync(Guid jobId, Guid userId)
    {
      return await CanUpdateJobAsync(jobId, userId);
    }
  }
}
