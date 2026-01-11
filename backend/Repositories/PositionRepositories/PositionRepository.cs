using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs.Position;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
  public class PositionRepository(ApplicationDbContext context) : IPositionRepository
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<Position> CreatePositionAsync(Position position)
    {
      _context.Positions.Add(position);
      await _context.SaveChangesAsync();
      return position;
    }

    public async Task<Position?> GetPositionByIdAsync(Guid id)
    {
      return await _context.Positions
          .Include(p => p.Status)
          .Include(p => p.Reviewer)
              .ThenInclude(r => r!.User)
          .Include(p => p.SelectedCandidate)
              .ThenInclude(c => c!.User)
          .Include(p => p.PositionSkills)
              .ThenInclude(ps => ps.Skill)
          .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<PositionResponseDto?> GetPositionDetailsByIdAsync(Guid id)
    {
      return await _context.Positions
          .Where(p => p.Id == id && !p.IsDeleted)
          .Select(p => new PositionResponseDto
          {
            Id = p.Id,
            Title = p.Title,
            NumberOfInterviews = p.NumberOfInterviews,
            ClosedReason = p.ClosedReason,
            SelectedCandidateId = p.SelectedCandidateId,
            SelectedCandidate = p.SelectedCandidate != null ? new PositionSelectedCandidateDto
            {
              Id = p.SelectedCandidate.Id,
              FullName = p.SelectedCandidate.FullName,
              Email = p.SelectedCandidate.User.Email
            } : null,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Status = new PositionStatusDto
            {
              Id = p.Status.Id,
              Status = p.Status.Status
            },
            Reviewer = p.Reviewer != null ? new PositionReviewerDto
            {
              Id = p.Reviewer.Id,
              FullName = p.Reviewer.FullName,
              Email = p.Reviewer.User.Email
            } : null,
            Skills = p.PositionSkills.Select(ps => new PositionSkillResponseDto
            {
              Id = ps.Skill.Id,
              SkillName = ps.Skill.SkillName
            }).ToList(),
            JobCount = p.Jobs.Count(j => !j.IsDeleted)
          })
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PositionListDto>> GetPositionsAsync(int page = 1, int pageSize = 10, string? search = null, Guid? statusId = null)
    {
      var query = _context.Positions
          .Where(p => !p.IsDeleted);

      if (!string.IsNullOrEmpty(search))
      {
        query = query.Where(p => p.Title.Contains(search));
      }

      if (statusId.HasValue)
      {
        query = query.Where(p => p.StatusId == statusId.Value);
      }

      return await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(p => new PositionListDto
          {
            Id = p.Id,
            Title = p.Title,
            Status = p.Status.Status,
            ReviewerName = p.Reviewer != null ? p.Reviewer.FullName : null,
            NumberOfInterviews = p.NumberOfInterviews,
            CreatedAt = p.CreatedAt,
            JobCount = p.Jobs.Count(j => !j.IsDeleted),
            SkillCount = p.PositionSkills.Count
          })
          .OrderByDescending(p => p.CreatedAt)
          .ToListAsync();
    }

    public async Task<Position> UpdatePositionAsync(Position position)
    {
      _context.Positions.Update(position);
      await _context.SaveChangesAsync();
      return position;
    }

    public async Task DeletePositionAsync(Guid id)
    {
      var position = await _context.Positions.FindAsync(id);
      if (position != null)
      {
        position.IsDeleted = true;
        position.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
      }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
      return await _context.Positions.AnyAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<int> GetPositionCountAsync(Guid? statusId = null)
    {
      var query = _context.Positions.Where(p => !p.IsDeleted);

      if (statusId.HasValue)
      {
        query = query.Where(p => p.StatusId == statusId.Value);
      }

      return await query.CountAsync();
    }
  }
}
