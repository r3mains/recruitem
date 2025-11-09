using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Consts;

namespace backend.Repositories;

public class ScreeningRepository : IScreeningRepository
{
  private readonly ApplicationDbContext _context;

  public ScreeningRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<JobApplication> ScreenResumeAsync(Guid jobApplicationId, decimal? score, string? comments, bool approved, Guid screenerId)
  {
    var application = await _context.JobApplications
      .Include(ja => ja.Status)
      .FirstOrDefaultAsync(ja => ja.Id == jobApplicationId);

    if (application == null)
      throw new InvalidOperationException("Job application not found");

    // Update application with screening results
    application.Score = score;
    application.UpdatedBy = screenerId;
    application.LastUpdated = DateTime.UtcNow;

    // Update status based on screening result
    var newStatus = approved ? backend.Consts.ApplicationStatus.Screening : backend.Consts.ApplicationStatus.Rejected;
    var statusEntity = await _context.ApplicationStatuses
      .FirstOrDefaultAsync(s => s.Status == newStatus);
    
    if (statusEntity != null && application.StatusId != statusEntity.Id)
    {
      var previousStatus = application.Status?.Status;
      application.StatusId = statusEntity.Id;

      // Add status history
      var statusHistory = new ApplicationStatusHistory
      {
        JobApplicationId = jobApplicationId,
        StatusId = statusEntity.Id,
        ChangedAt = DateTime.UtcNow,
        ChangedBy = screenerId,
        Note = comments
      };
      _context.ApplicationStatusHistory.Add(statusHistory);
    }

    // Add comment if provided
    if (!string.IsNullOrEmpty(comments))
    {
      var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == screenerId);
      if (employee != null)
      {
        var comment = new Comment
        {
          JobApplicationId = jobApplicationId,
          CommenterId = employee.Id,
          CommentText = comments,
          CreatedAt = DateTime.UtcNow
        };
        _context.Comments.Add(comment);
      }
    }

    await _context.SaveChangesAsync();
    return application;
  }

  public async Task<IEnumerable<JobApplication>> GetApplicationsForScreeningAsync(Guid? positionId = null, string? search = null, int page = 1, int pageSize = 10)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.User)
      .Include(ja => ja.Job)
        .ThenInclude(j => j.Position)
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted)
      .AsQueryable();

    // Filter by position if specified
    if (positionId.HasValue)
    {
      query = query.Where(ja => ja.Job.PositionId == positionId.Value);
    }

    // Filter for applications that need screening (Applied status)
    var appliedStatus = await _context.ApplicationStatuses
      .FirstOrDefaultAsync(s => s.Status == backend.Consts.ApplicationStatus.Applied);
    if (appliedStatus != null)
    {
      query = query.Where(ja => ja.StatusId == appliedStatus.Id);
    }

    // Search functionality
    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(ja =>
        ja.Candidate.FullName!.ToLower().Contains(search.ToLower()) ||
        ja.Candidate.User.Email!.ToLower().Contains(search.ToLower()) ||
        ja.Job.Title.ToLower().Contains(search.ToLower()));
    }

    return await query
      .OrderBy(ja => ja.AppliedAt ?? DateTime.UtcNow)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<Comment> AddCommentAsync(Guid jobApplicationId, string comment, Guid commenterId)
  {
    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == commenterId);
    if (employee == null)
      throw new InvalidOperationException("Employee not found");

    var commentEntity = new Comment
    {
      JobApplicationId = jobApplicationId,
      CommenterId = employee.Id,
      CommentText = comment,
      CreatedAt = DateTime.UtcNow
    };

    _context.Comments.Add(commentEntity);
    await _context.SaveChangesAsync();

    return commentEntity;
  }

  public async Task<IEnumerable<Comment>> GetApplicationCommentsAsync(Guid jobApplicationId)
  {
    return await _context.Comments
      .Include(c => c.Commenter)
        .ThenInclude(e => e.User)
      .Where(c => c.JobApplicationId == jobApplicationId)
      .OrderBy(c => c.CreatedAt)
      .ToListAsync();
  }

  public async Task UpdateCandidateSkillsAsync(Guid candidateId, List<(Guid SkillId, int? YearsOfExperience, string? ProficiencyLevel)> skills, Guid updatedBy)
  {
    // Remove existing skills
    var existingSkills = await _context.CandidateSkills
      .Where(cs => cs.CandidateId == candidateId)
      .ToListAsync();
    _context.CandidateSkills.RemoveRange(existingSkills);

    // Add updated skills
    foreach (var (skillId, yearsOfExperience, proficiencyLevel) in skills)
    {
      var candidateSkill = new CandidateSkill
      {
        CandidateId = candidateId,
        SkillId = skillId,
        YearOfExperience = yearsOfExperience
      };
      _context.CandidateSkills.Add(candidateSkill);
    }

    // Update candidate's UpdatedAt timestamp
    var candidate = await _context.Candidates.FindAsync(candidateId);
    if (candidate != null)
    {
      candidate.UpdatedAt = DateTime.UtcNow;
    }

    await _context.SaveChangesAsync();
  }

  public async Task<IEnumerable<CandidateSkill>> GetCandidateSkillsForScreeningAsync(Guid candidateId)
  {
    return await _context.CandidateSkills
      .Include(cs => cs.Skill)
      .Where(cs => cs.CandidateId == candidateId)
      .OrderBy(cs => cs.Skill.SkillName)
      .ToListAsync();
  }

  public async Task<Position> AssignReviewerToPositionAsync(Guid positionId, Guid reviewerId)
  {
    var position = await _context.Positions.FindAsync(positionId);
    if (position == null)
      throw new InvalidOperationException("Position not found");

    var reviewer = await _context.Employees.FindAsync(reviewerId);
    if (reviewer == null)
      throw new InvalidOperationException("Reviewer not found");

    position.ReviewerId = reviewerId;
    position.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return position;
  }

  public async Task<IEnumerable<Position>> GetPositionsForReviewerAssignmentAsync()
  {
    return await _context.Positions
      .Include(p => p.Status)
      .Include(p => p.Reviewer)
        .ThenInclude(r => r!.User)
      .Where(p => !p.IsDeleted)
      .OrderBy(p => p.Title)
      .ToListAsync();
  }

  public async Task<JobApplication> ShortlistCandidateAsync(Guid jobApplicationId, string? comments, Guid shortlistedBy)
  {
    var application = await _context.JobApplications
      .Include(ja => ja.Status)
      .FirstOrDefaultAsync(ja => ja.Id == jobApplicationId);

    if (application == null)
      throw new InvalidOperationException("Job application not found");

    // Update status to shortlisted
    var shortlistedStatus = await _context.ApplicationStatuses
      .FirstOrDefaultAsync(s => s.Status == backend.Consts.ApplicationStatus.Shortlisted);
    
    if (shortlistedStatus != null)
    {
      var previousStatus = application.Status?.Status;
      application.StatusId = shortlistedStatus.Id;
      application.UpdatedBy = shortlistedBy;
      application.LastUpdated = DateTime.UtcNow;

      // Add status history
      var statusHistory = new ApplicationStatusHistory
      {
        JobApplicationId = jobApplicationId,
        StatusId = shortlistedStatus.Id,
        ChangedAt = DateTime.UtcNow,
        ChangedBy = shortlistedBy,
        Note = comments ?? "Candidate shortlisted for interview"
      };
      _context.ApplicationStatusHistory.Add(statusHistory);
    }

    // Add comment if provided
    if (!string.IsNullOrEmpty(comments))
    {
      var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == shortlistedBy);
      if (employee != null)
      {
        var comment = new Comment
        {
          JobApplicationId = jobApplicationId,
          CommenterId = employee.Id,
          CommentText = comments,
          CreatedAt = DateTime.UtcNow
        };
        _context.Comments.Add(comment);
      }
    }

    await _context.SaveChangesAsync();
    return application;
  }

  public async Task<IEnumerable<JobApplication>> GetShortlistedCandidatesAsync(Guid? positionId = null, int page = 1, int pageSize = 10)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.User)
      .Include(ja => ja.Job)
        .ThenInclude(j => j.Position)
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted)
      .AsQueryable();

    // Filter by position if specified
    if (positionId.HasValue)
    {
      query = query.Where(ja => ja.Job.PositionId == positionId.Value);
    }

    // Filter for shortlisted applications
    var shortlistedStatus = await _context.ApplicationStatuses
      .FirstOrDefaultAsync(s => s.Status == backend.Consts.ApplicationStatus.Shortlisted);
    if (shortlistedStatus != null)
    {
      query = query.Where(ja => ja.StatusId == shortlistedStatus.Id);
    }

    return await query
      .OrderByDescending(ja => ja.LastUpdated ?? DateTime.UtcNow)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<Dictionary<string, int>> GetScreeningStatsAsync(Guid? positionId = null, DateTime? fromDate = null, DateTime? toDate = null)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted)
      .AsQueryable();

    if (positionId.HasValue)
    {
      query = query.Where(ja => ja.Job.PositionId == positionId.Value);
    }

    if (fromDate.HasValue)
    {
      query = query.Where(ja => ja.AppliedAt >= fromDate.Value);
    }

    if (toDate.HasValue)
    {
      query = query.Where(ja => ja.AppliedAt <= toDate.Value);
    }

    var stats = await query
      .GroupBy(ja => ja.Status.Status)
      .Select(g => new { Status = g.Key, Count = g.Count() })
      .ToListAsync();

    return stats.ToDictionary(s => s.Status, s => s.Count);
  }

  public async Task<int> GetPendingScreeningCountAsync(Guid? reviewerId = null)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted)
      .AsQueryable();

    // Filter for applied status (pending screening)
    var appliedStatus = await _context.ApplicationStatuses
      .FirstOrDefaultAsync(s => s.Status == backend.Consts.ApplicationStatus.Applied);
    if (appliedStatus != null)
    {
      query = query.Where(ja => ja.StatusId == appliedStatus.Id);
    }

    // Filter by reviewer if specified
    if (reviewerId.HasValue)
    {
      query = query.Where(ja => ja.Job.Position!.ReviewerId == reviewerId.Value);
    }

    return await query.CountAsync();
  }
}
