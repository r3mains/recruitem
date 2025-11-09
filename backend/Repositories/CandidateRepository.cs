using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Repositories.IRepositories;

namespace backend.Repositories;

public class CandidateRepository : ICandidateRepository
{
  private readonly ApplicationDbContext _context;

  public CandidateRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<backend.Models.Candidate>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10)
  {
    var query = _context.Candidates
      .Include(c => c.User)
      .Include(c => c.Address!)
        .ThenInclude(a => a.City!)
          .ThenInclude(c => c.State)
      .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
      .Include(c => c.JobApplications.Where(ja => !ja.IsDeleted))
      .Where(c => !c.IsDeleted)
      .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(c =>
        c.FullName!.ToLower().Contains(search.ToLower()) ||
        c.User.Email!.ToLower().Contains(search.ToLower()) ||
        c.ContactNumber!.ToLower().Contains(search.ToLower()));
    }

    return await query
      .OrderBy(c => c.FullName)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<backend.Models.Candidate?> GetByIdAsync(Guid id)
  {
    return await _context.Candidates
      .Include(c => c.User)
      .Include(c => c.Address!)
        .ThenInclude(a => a.City!)
          .ThenInclude(c => c.State)
      .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
      .Include(c => c.CandidateQualifications)
        .ThenInclude(cq => cq.Qualification)
      .Include(c => c.JobApplications.Where(ja => !ja.IsDeleted))
      .Include(c => c.Documents)
      .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
  }

  public async Task<backend.Models.Candidate?> GetByUserIdAsync(Guid userId)
  {
    return await _context.Candidates
      .Include(c => c.User)
      .Include(c => c.Address!)
        .ThenInclude(a => a.City!)
          .ThenInclude(c => c.State)
      .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
      .Include(c => c.CandidateQualifications)
        .ThenInclude(cq => cq.Qualification)
      .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
  }

  public async Task<backend.Models.Candidate?> GetByEmailAsync(string email)
  {
    return await _context.Candidates
      .Include(c => c.User)
      .FirstOrDefaultAsync(c => c.User.Email == email && !c.IsDeleted);
  }

  public async Task<backend.Models.Candidate> CreateAsync(backend.Models.Candidate candidate)
  {
    _context.Candidates.Add(candidate);
    await _context.SaveChangesAsync();
    return candidate;
  }

  public async Task<backend.Models.Candidate> UpdateAsync(backend.Models.Candidate candidate)
  {
    candidate.UpdatedAt = DateTime.UtcNow;
    _context.Candidates.Update(candidate);
    await _context.SaveChangesAsync();
    return candidate;
  }

  public async Task DeleteAsync(Guid id)
  {
    var candidate = await _context.Candidates.FindAsync(id);
    if (candidate != null)
    {
      candidate.IsDeleted = true;
      candidate.UpdatedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
    }
  }

  public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
  {
    var query = _context.Candidates
      .Include(c => c.User)
      .Where(c => c.User.Email == email && !c.IsDeleted);

    if (excludeId.HasValue)
    {
      query = query.Where(c => c.Id != excludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<int> GetCountAsync(string? search = null)
  {
    var query = _context.Candidates
      .Include(c => c.User)
      .Where(c => !c.IsDeleted)
      .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(c =>
        c.FullName!.ToLower().Contains(search.ToLower()) ||
        c.User.Email!.ToLower().Contains(search.ToLower()) ||
        c.ContactNumber!.ToLower().Contains(search.ToLower()));
    }

    return await query.CountAsync();
  }

  public async Task<IEnumerable<backend.Models.CandidateSkill>> GetCandidateSkillsAsync(Guid candidateId)
  {
    return await _context.CandidateSkills
      .Include(cs => cs.Skill)
      .Where(cs => cs.CandidateId == candidateId)
      .ToListAsync();
  }

  public async Task<backend.Models.CandidateSkill> AddSkillAsync(backend.Models.CandidateSkill candidateSkill)
  {
    _context.CandidateSkills.Add(candidateSkill);
    await _context.SaveChangesAsync();
    return candidateSkill;
  }

  public async Task UpdateSkillAsync(backend.Models.CandidateSkill candidateSkill)
  {
    _context.CandidateSkills.Update(candidateSkill);
    await _context.SaveChangesAsync();
  }

  public async Task RemoveSkillAsync(Guid candidateId, Guid skillId)
  {
    var candidateSkill = await _context.CandidateSkills
      .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId && cs.SkillId == skillId);

    if (candidateSkill != null)
    {
      _context.CandidateSkills.Remove(candidateSkill);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<IEnumerable<backend.Models.CandidateQualification>> GetCandidateQualificationsAsync(Guid candidateId)
  {
    return await _context.CandidateQualifications
      .Include(cq => cq.Qualification)
      .Where(cq => cq.CandidateId == candidateId)
      .ToListAsync();
  }

  public async Task<backend.Models.CandidateQualification> AddQualificationAsync(backend.Models.CandidateQualification candidateQualification)
  {
    _context.CandidateQualifications.Add(candidateQualification);
    await _context.SaveChangesAsync();
    return candidateQualification;
  }

  public async Task UpdateQualificationAsync(backend.Models.CandidateQualification candidateQualification)
  {
    _context.CandidateQualifications.Update(candidateQualification);
    await _context.SaveChangesAsync();
  }

  public async Task RemoveQualificationAsync(Guid candidateId, Guid qualificationId)
  {
    var candidateQualification = await _context.CandidateQualifications
      .FirstOrDefaultAsync(cq => cq.CandidateId == candidateId && cq.QualificationId == qualificationId);

    if (candidateQualification != null)
    {
      _context.CandidateQualifications.Remove(candidateQualification);
      await _context.SaveChangesAsync();
    }
  }

  public async Task RemoveAllSkillsAsync(Guid candidateId)
  {
    var candidateSkills = await _context.CandidateSkills
      .Where(cs => cs.CandidateId == candidateId)
      .ToListAsync();

    if (candidateSkills.Any())
    {
      _context.CandidateSkills.RemoveRange(candidateSkills);
      await _context.SaveChangesAsync();
    }
  }

  public async Task RemoveAllQualificationsAsync(Guid candidateId)
  {
    var candidateQualifications = await _context.CandidateQualifications
      .Where(cq => cq.CandidateId == candidateId)
      .ToListAsync();

    if (candidateQualifications.Any())
    {
      _context.CandidateQualifications.RemoveRange(candidateQualifications);
      await _context.SaveChangesAsync();
    }
  }
}
