using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs.Skill;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
  public class SkillRepository(ApplicationDbContext context) : ISkillRepository
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<Skill> CreateSkillAsync(Skill skill)
    {
      _context.Skills.Add(skill);
      await _context.SaveChangesAsync();
      return skill;
    }

    public async Task<Skill?> GetSkillByIdAsync(Guid id)
    {
      return await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SkillResponseDto?> GetSkillDetailsByIdAsync(Guid id)
    {
      return await _context.Skills
          .Where(s => s.Id == id)
          .Select(s => new SkillResponseDto
          {
            Id = s.Id,
            SkillName = s.SkillName,
            PositionCount = s.PositionSkills.Count,
            JobCount = s.JobSkills.Count,
            CandidateCount = s.CandidateSkills.Count
          })
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SkillListDto>> GetSkillsAsync(int page = 1, int pageSize = 10, string? search = null)
    {
      var query = _context.Skills.AsQueryable();

      if (!string.IsNullOrEmpty(search))
      {
        query = query.Where(s => s.SkillName.Contains(search));
      }

      return await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(s => new SkillListDto
          {
            Id = s.Id,
            SkillName = s.SkillName,
            UsageCount = s.PositionSkills.Count + s.JobSkills.Count + s.CandidateSkills.Count
          })
          .OrderBy(s => s.SkillName)
          .ToListAsync();
    }

    public async Task<Skill> UpdateSkillAsync(Skill skill)
    {
      _context.Skills.Update(skill);
      await _context.SaveChangesAsync();
      return skill;
    }

    public async Task DeleteSkillAsync(Guid id)
    {
      var skill = await _context.Skills.FindAsync(id);
      if (skill != null)
      {
        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
      return await _context.Skills.AnyAsync(s => s.Id == id);
    }

    public async Task<int> GetSkillCountAsync()
    {
      return await _context.Skills.CountAsync();
    }

    public async Task<bool> SkillNameExistsAsync(string skillName, Guid? excludeId = null)
    {
      var query = _context.Skills.Where(s => s.SkillName.ToLower() == skillName.ToLower());

      if (excludeId.HasValue)
      {
        query = query.Where(s => s.Id != excludeId.Value);
      }

      return await query.AnyAsync();
    }
  }
}
