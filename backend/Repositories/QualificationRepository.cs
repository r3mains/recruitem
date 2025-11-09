using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Repositories.IRepositories;

namespace backend.Repositories;

public class QualificationRepository : IQualificationRepository
{
  private readonly ApplicationDbContext _context;

  public QualificationRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<backend.Models.Qualification>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10)
  {
    var query = _context.Qualifications
      .Include(q => q.JobQualifications)
      .Include(q => q.CandidateQualifications)
      .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(q => q.QualificationName.ToLower().Contains(search.ToLower()));
    }

    return await query
      .OrderBy(q => q.QualificationName)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<backend.Models.Qualification?> GetByIdAsync(Guid id)
  {
    return await _context.Qualifications
      .Include(q => q.JobQualifications)
      .Include(q => q.CandidateQualifications)
      .FirstOrDefaultAsync(q => q.Id == id);
  }

  public async Task<backend.Models.Qualification> CreateAsync(backend.Models.Qualification qualification)
  {
    _context.Qualifications.Add(qualification);
    await _context.SaveChangesAsync();
    return qualification;
  }

  public async Task<backend.Models.Qualification> UpdateAsync(backend.Models.Qualification qualification)
  {
    _context.Qualifications.Update(qualification);
    await _context.SaveChangesAsync();
    return qualification;
  }

  public async Task DeleteAsync(Guid id)
  {
    var qualification = await _context.Qualifications.FindAsync(id);
    if (qualification != null)
    {
      _context.Qualifications.Remove(qualification);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
  {
    var query = _context.Qualifications.Where(q => q.QualificationName.ToLower() == name.ToLower());

    if (excludeId.HasValue)
    {
      query = query.Where(q => q.Id != excludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> IsInUseAsync(Guid id)
  {
    var hasJobs = await _context.JobQualifications.AnyAsync(jq => jq.QualificationId == id);
    var hasCandidates = await _context.CandidateQualifications.AnyAsync(cq => cq.QualificationId == id);

    return hasJobs || hasCandidates;
  }

  public async Task<int> GetCountAsync(string? search = null)
  {
    var query = _context.Qualifications.AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(q => q.QualificationName.ToLower().Contains(search.ToLower()));
    }

    return await query.CountAsync();
  }

  public async Task<int> GetInUseCountAsync()
  {
    return await _context.Qualifications
      .Where(q => q.JobQualifications.Any() || q.CandidateQualifications.Any())
      .CountAsync();
  }
}
