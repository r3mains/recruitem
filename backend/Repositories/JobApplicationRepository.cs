using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class JobApplicationRepository(AppDbContext context) : IJobApplicationRepository
{
  public async Task<List<JobApplication>> GetAll(Guid? jobId = null, Guid? candidateId = null, Guid? statusId = null)
  {
    var query = context.JobApplications
      .Include(ja => ja.Job)
      .Include(ja => ja.Candidate)
      .ThenInclude(c => c!.User)
      .AsQueryable();

    if (jobId.HasValue)
      query = query.Where(ja => ja.JobId == jobId);

    if (candidateId.HasValue)
      query = query.Where(ja => ja.CandidateId == candidateId);

    if (statusId.HasValue)
      query = query.Where(ja => ja.StatusId == statusId);

    return await query.OrderByDescending(ja => ja.AppliedAt).ToListAsync();
  }

  public async Task<JobApplication?> GetById(Guid id)
  {
    return await context.JobApplications
      .Include(ja => ja.Job)
      .Include(ja => ja.Candidate)
      .ThenInclude(c => c!.User)
      .FirstOrDefaultAsync(ja => ja.Id == id);
  }

  public async Task<JobApplication> Add(JobApplication jobApplication)
  {
    jobApplication.Id = Guid.NewGuid();
    jobApplication.AppliedAt = DateTime.UtcNow;
    context.JobApplications.Add(jobApplication);
    await context.SaveChangesAsync();
    return jobApplication;
  }

  public async Task<JobApplication?> Update(Guid id, JobApplication jobApplication)
  {
    var existing = await context.JobApplications.FindAsync(id);
    if (existing == null) return null;

    existing.StatusId = jobApplication.StatusId;
    existing.Notes = jobApplication.Notes;
    existing.ReviewedAt = DateTime.UtcNow;
    existing.ReviewedBy = jobApplication.ReviewedBy;

    await context.SaveChangesAsync();
    return existing;
  }

  public async Task<bool> Delete(Guid id)
  {
    var jobApplication = await context.JobApplications.FindAsync(id);
    if (jobApplication == null) return false;

    context.JobApplications.Remove(jobApplication);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> ExistsForJobAndCandidate(Guid jobId, Guid candidateId)
  {
    return await context.JobApplications
      .AnyAsync(ja => ja.JobId == jobId && ja.CandidateId == candidateId);
  }
}
