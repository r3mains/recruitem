using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Repositories.IRepositories;

namespace backend.Repositories;

public class JobTypeRepository : IJobTypeRepository
{
  private readonly ApplicationDbContext _context;

  public JobTypeRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<backend.Models.JobType>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10)
  {
    var query = _context.JobTypes
      .Include(jt => jt.Jobs.Where(j => !j.IsDeleted))
      .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(jt => jt.Type.ToLower().Contains(search.ToLower()));
    }

    return await query
      .OrderBy(jt => jt.Type)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<backend.Models.JobType?> GetByIdAsync(Guid id)
  {
    return await _context.JobTypes
      .Include(jt => jt.Jobs.Where(j => !j.IsDeleted))
      .FirstOrDefaultAsync(jt => jt.Id == id);
  }

  public async Task<backend.Models.JobType> CreateAsync(backend.Models.JobType jobType)
  {
    _context.JobTypes.Add(jobType);
    await _context.SaveChangesAsync();
    return jobType;
  }

  public async Task<backend.Models.JobType> UpdateAsync(backend.Models.JobType jobType)
  {
    _context.JobTypes.Update(jobType);
    await _context.SaveChangesAsync();
    return jobType;
  }

  public async Task DeleteAsync(Guid id)
  {
    var jobType = await _context.JobTypes.FindAsync(id);
    if (jobType != null)
    {
      _context.JobTypes.Remove(jobType);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
  {
    var query = _context.JobTypes.Where(jt => jt.Type.ToLower() == name.ToLower());

    if (excludeId.HasValue)
    {
      query = query.Where(jt => jt.Id != excludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> IsInUseAsync(Guid id)
  {
    return await _context.Jobs.AnyAsync(j => j.JobTypeId == id && !j.IsDeleted);
  }

  public async Task<int> GetCountAsync(string? search = null)
  {
    var query = _context.JobTypes.AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(jt => jt.Type.ToLower().Contains(search.ToLower()));
    }

    return await query.CountAsync();
  }
}
