using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Repositories.IRepositories;

namespace backend.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
  private readonly ApplicationDbContext _context;

  public JobApplicationRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<backend.Models.JobApplication>> GetAllAsync(
    string? search = null,
    int page = 1,
    int pageSize = 10,
    Guid? jobId = null,
    Guid? candidateId = null,
    Guid? statusId = null)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Job)
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.User)
      .Include(ja => ja.Status)
      .Include(ja => ja.CreatedByUser)
      .Include(ja => ja.UpdatedByUser)
      .Where(ja => !ja.IsDeleted)
      .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(ja =>
        ja.Job.Title.ToLower().Contains(search.ToLower()) ||
        ja.Candidate.FullName!.ToLower().Contains(search.ToLower()) ||
        ja.Candidate.User.Email!.ToLower().Contains(search.ToLower()));
    }

    if (jobId.HasValue)
    {
      query = query.Where(ja => ja.JobId == jobId.Value);
    }

    if (candidateId.HasValue)
    {
      query = query.Where(ja => ja.CandidateId == candidateId.Value);
    }

    if (statusId.HasValue)
    {
      query = query.Where(ja => ja.StatusId == statusId.Value);
    }

    return await query
      .OrderByDescending(ja => ja.AppliedAt)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<backend.Models.JobApplication?> GetByIdAsync(Guid id)
  {
    return await _context.JobApplications
      .Include(ja => ja.Job)
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.User)
      .Include(ja => ja.Status)
      .Include(ja => ja.CreatedByUser)
      .Include(ja => ja.UpdatedByUser)
      .Include(ja => ja.ApplicationDocuments)
        .ThenInclude(ad => ad.Document)
          .ThenInclude(d => d.DocumentType)
      .Include(ja => ja.Comments)
        .ThenInclude(c => c.Commenter)
      .Include(ja => ja.StatusHistory)
        .ThenInclude(sh => sh.Status)
      .Include(ja => ja.StatusHistory)
        .ThenInclude(sh => sh.ChangedByUser)
      .FirstOrDefaultAsync(ja => ja.Id == id && !ja.IsDeleted);
  }

  public async Task<backend.Models.JobApplication> CreateAsync(backend.Models.JobApplication jobApplication)
  {
    jobApplication.AppliedAt = DateTime.UtcNow;
    jobApplication.LastUpdated = DateTime.UtcNow;
    _context.JobApplications.Add(jobApplication);
    await _context.SaveChangesAsync();
    return jobApplication;
  }

  public async Task<backend.Models.JobApplication> UpdateAsync(backend.Models.JobApplication jobApplication)
  {
    jobApplication.LastUpdated = DateTime.UtcNow;
    _context.JobApplications.Update(jobApplication);
    await _context.SaveChangesAsync();
    return jobApplication;
  }

  public async Task DeleteAsync(Guid id)
  {
    var jobApplication = await _context.JobApplications.FindAsync(id);
    if (jobApplication != null)
    {
      jobApplication.IsDeleted = true;
      jobApplication.LastUpdated = DateTime.UtcNow;
      await _context.SaveChangesAsync();
    }
  }

  public async Task<bool> HasAppliedAsync(Guid candidateId, Guid jobId)
  {
    return await _context.JobApplications
      .AnyAsync(ja => ja.CandidateId == candidateId && ja.JobId == jobId && !ja.IsDeleted);
  }

  public async Task<int> GetCountAsync(string? search = null, Guid? jobId = null, Guid? candidateId = null)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Job)
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.User)
      .Where(ja => !ja.IsDeleted)
      .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(ja =>
        ja.Job.Title.ToLower().Contains(search.ToLower()) ||
        ja.Candidate.FullName!.ToLower().Contains(search.ToLower()) ||
        ja.Candidate.User.Email!.ToLower().Contains(search.ToLower()));
    }

    if (jobId.HasValue)
    {
      query = query.Where(ja => ja.JobId == jobId.Value);
    }

    if (candidateId.HasValue)
    {
      query = query.Where(ja => ja.CandidateId == candidateId.Value);
    }

    return await query.CountAsync();
  }

  public async Task<IEnumerable<backend.Models.JobApplication>> GetByIdsAsync(IEnumerable<Guid> applicationIds)
  {
    return await _context.JobApplications
      .Include(ja => ja.Job)
      .Include(ja => ja.Candidate)
      .Include(ja => ja.Status)
      .Where(ja => applicationIds.Contains(ja.Id) && !ja.IsDeleted)
      .ToListAsync();
  }

  public async Task UpdateMultipleAsync(IEnumerable<backend.Models.JobApplication> applications)
  {
    foreach (var application in applications)
    {
      application.LastUpdated = DateTime.UtcNow;
    }

    _context.JobApplications.UpdateRange(applications);
    await _context.SaveChangesAsync();
  }

  public async Task<Dictionary<string, int>> GetApplicationStatsByJobAsync(Guid jobId)
  {
    return await _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => ja.JobId == jobId && !ja.IsDeleted)
      .GroupBy(ja => ja.Status.Status)
      .ToDictionaryAsync(g => g.Key, g => g.Count());
  }

  public async Task<Dictionary<string, int>> GetApplicationStatsByCandidateAsync(Guid candidateId)
  {
    return await _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => ja.CandidateId == candidateId && !ja.IsDeleted)
      .GroupBy(ja => ja.Status.Status)
      .ToDictionaryAsync(g => g.Key, g => g.Count());
  }

  public async Task AddStatusHistoryAsync(backend.Models.ApplicationStatusHistory statusHistory)
  {
    _context.ApplicationStatusHistory.Add(statusHistory);
    await _context.SaveChangesAsync();
  }
}
