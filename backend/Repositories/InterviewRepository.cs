using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs.Interview;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
  public class InterviewRepository : IInterviewRepository
  {
    private readonly ApplicationDbContext _context;

    public InterviewRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<InterviewDto>> GetAllInterviewsAsync()
    {
      return await _context.Interviews
          .Include(i => i.JobApplication)
              .ThenInclude(ja => ja.Candidate)
                  .ThenInclude(c => c.User)
          .Include(i => i.JobApplication)
              .ThenInclude(ja => ja.Job)
          .Include(i => i.InterviewType)
          .Include(i => i.Status)
          .Where(i => !i.IsDeleted)
          .Select(i => new InterviewDto
          {
            Id = i.Id,
            JobApplicationId = i.JobApplicationId,
            CandidateName = i.JobApplication.Candidate.User.UserName ?? "Unknown",
            JobTitle = i.JobApplication.Job.Title,
            InterviewTypeId = i.InterviewTypeId,
            InterviewType = i.InterviewType.Type,
            StatusId = i.StatusId,
            Status = i.Status != null ? i.Status.Status : null,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
          })
          .ToListAsync();
    }

    public async Task<InterviewDto?> GetInterviewByIdAsync(Guid id)
    {
      return await _context.Interviews
          .Include(i => i.JobApplication)
              .ThenInclude(ja => ja.Candidate)
                  .ThenInclude(c => c.User)
          .Include(i => i.JobApplication)
              .ThenInclude(ja => ja.Job)
          .Include(i => i.InterviewType)
          .Include(i => i.Status)
          .Where(i => i.Id == id && !i.IsDeleted)
          .Select(i => new InterviewDto
          {
            Id = i.Id,
            JobApplicationId = i.JobApplicationId,
            CandidateName = i.JobApplication.Candidate.User.UserName ?? "Unknown",
            JobTitle = i.JobApplication.Job.Title,
            InterviewTypeId = i.InterviewTypeId,
            InterviewType = i.InterviewType.Type,
            StatusId = i.StatusId,
            Status = i.Status != null ? i.Status.Status : null,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
          })
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<InterviewDto>> GetInterviewsByJobApplicationIdAsync(Guid jobApplicationId)
    {
      return await _context.Interviews
          .Include(i => i.JobApplication)
              .ThenInclude(ja => ja.Candidate)
                  .ThenInclude(c => c.User)
          .Include(i => i.JobApplication)
              .ThenInclude(ja => ja.Job)
          .Include(i => i.InterviewType)
          .Include(i => i.Status)
          .Where(i => i.JobApplicationId == jobApplicationId && !i.IsDeleted)
          .Select(i => new InterviewDto
          {
            Id = i.Id,
            JobApplicationId = i.JobApplicationId,
            CandidateName = i.JobApplication.Candidate.User.UserName ?? "Unknown",
            JobTitle = i.JobApplication.Job.Title,
            InterviewTypeId = i.InterviewTypeId,
            InterviewType = i.InterviewType.Type,
            StatusId = i.StatusId,
            Status = i.Status != null ? i.Status.Status : null,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
          })
          .ToListAsync();
    }

    public async Task<InterviewDto> CreateInterviewAsync(CreateInterviewDto dto, Guid createdBy)
    {
      var interview = new Interview
      {
        Id = Guid.NewGuid(),
        JobApplicationId = dto.JobApplicationId,
        InterviewTypeId = dto.InterviewTypeId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Interviews.Add(interview);

      foreach (var interviewerId in dto.InterviewerIds)
      {
        var interviewer = new Interviewer
        {
          Id = Guid.NewGuid(),
          InterviewId = interview.Id,
          InterviewerId = interviewerId
        };
        _context.Interviewers.Add(interviewer);
      }

      await _context.SaveChangesAsync();
      return (await GetInterviewByIdAsync(interview.Id))!;
    }

    public async Task<InterviewDto?> UpdateInterviewAsync(Guid id, UpdateInterviewDto dto)
    {
      var interview = await _context.Interviews.FindAsync(id);
      if (interview == null || interview.IsDeleted) return null;

      if (dto.InterviewTypeId.HasValue)
        interview.InterviewTypeId = dto.InterviewTypeId.Value;
      if (dto.StatusId.HasValue)
        interview.StatusId = dto.StatusId.Value;

      interview.UpdatedAt = DateTime.UtcNow;

      if (dto.InterviewerIds != null)
      {
        var existingInterviewers = await _context.Interviewers
            .Where(i => i.InterviewId == id).ToListAsync();
        _context.Interviewers.RemoveRange(existingInterviewers);

        foreach (var interviewerId in dto.InterviewerIds)
        {
          var interviewer = new Interviewer
          {
            Id = Guid.NewGuid(),
            InterviewId = id,
            InterviewerId = interviewerId
          };
          _context.Interviewers.Add(interviewer);
        }
      }

      await _context.SaveChangesAsync();
      return await GetInterviewByIdAsync(id);
    }

    public async Task<bool> DeleteInterviewAsync(Guid id)
    {
      var interview = await _context.Interviews.FindAsync(id);
      if (interview == null || interview.IsDeleted) return false;

      interview.IsDeleted = true;
      interview.UpdatedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<IEnumerable<InterviewScheduleDto>> GetSchedulesByInterviewIdAsync(Guid interviewId)
    {
      return await _context.InterviewSchedules
          .Include(s => s.Status)
          .Include(s => s.CreatedByEmployee)
          .Where(s => s.InterviewId == interviewId)
          .Select(s => new InterviewScheduleDto
          {
            Id = s.Id,
            InterviewId = s.InterviewId,
            ScheduledAt = s.ScheduledAt,
            Location = s.Location,
            MeetingLink = s.MeetingLink,
            StatusId = s.StatusId,
            Status = s.Status.Status,
            CreatedBy = s.CreatedBy,
            CreatedByName = s.CreatedByEmployee.FullName ?? "Unknown",
            CreatedAt = s.CreatedAt
          })
          .ToListAsync();
    }

    public async Task<InterviewScheduleDto?> GetScheduleByIdAsync(Guid id)
    {
      return await _context.InterviewSchedules
          .Include(s => s.Status)
          .Include(s => s.CreatedByEmployee)
          .Where(s => s.Id == id)
          .Select(s => new InterviewScheduleDto
          {
            Id = s.Id,
            InterviewId = s.InterviewId,
            ScheduledAt = s.ScheduledAt,
            Location = s.Location,
            MeetingLink = s.MeetingLink,
            StatusId = s.StatusId,
            Status = s.Status.Status,
            CreatedBy = s.CreatedBy,
            CreatedByName = s.CreatedByEmployee.FullName ?? "Unknown",
            CreatedAt = s.CreatedAt
          })
          .FirstOrDefaultAsync();
    }

    public async Task<InterviewScheduleDto> CreateScheduleAsync(CreateInterviewScheduleDto dto, Guid createdBy)
    {
      var defaultStatus = await _context.ScheduleStatuses
          .FirstOrDefaultAsync(s => s.Status == Consts.ScheduleStatus.Scheduled);

      var schedule = new InterviewSchedule
      {
        Id = Guid.NewGuid(),
        InterviewId = dto.InterviewId,
        ScheduledAt = dto.ScheduledAt,
        Location = dto.Location,
        MeetingLink = dto.MeetingLink,
        StatusId = defaultStatus?.Id ?? Guid.NewGuid(),
        CreatedBy = createdBy,
        CreatedAt = DateTime.UtcNow
      };

      _context.InterviewSchedules.Add(schedule);
      await _context.SaveChangesAsync();
      return (await GetScheduleByIdAsync(schedule.Id))!;
    }

    public async Task<InterviewScheduleDto?> UpdateScheduleAsync(Guid id, UpdateInterviewScheduleDto dto)
    {
      var schedule = await _context.InterviewSchedules.FindAsync(id);
      if (schedule == null) return null;

      if (dto.ScheduledAt.HasValue)
        schedule.ScheduledAt = dto.ScheduledAt.Value;
      if (dto.Location != null)
        schedule.Location = dto.Location;
      if (dto.MeetingLink != null)
        schedule.MeetingLink = dto.MeetingLink;
      if (dto.StatusId.HasValue)
        schedule.StatusId = dto.StatusId.Value;

      await _context.SaveChangesAsync();
      return await GetScheduleByIdAsync(id);
    }

    public async Task<bool> DeleteScheduleAsync(Guid id)
    {
      var schedule = await _context.InterviewSchedules.FindAsync(id);
      if (schedule == null) return false;

      _context.InterviewSchedules.Remove(schedule);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<IEnumerable<InterviewFeedbackDto>> GetFeedbackByInterviewIdAsync(Guid interviewId)
    {
      return await _context.InterviewFeedbacks
          .Include(f => f.Skill)
          .Include(f => f.FeedbackByEmployee)
          .Where(f => f.InterviewId == interviewId)
          .Select(f => new InterviewFeedbackDto
          {
            Id = f.Id,
            InterviewId = f.InterviewId,
            ForSkill = f.ForSkill,
            SkillName = f.Skill.SkillName,
            FeedbackBy = f.FeedbackBy,
            FeedbackByName = f.FeedbackByEmployee.FullName ?? "Unknown",
            Rating = f.Rating,
            Feedback = f.Feedback,
            CreatedAt = f.CreatedAt
          })
          .ToListAsync();
    }

    public async Task<InterviewFeedbackDto?> GetFeedbackByIdAsync(Guid id)
    {
      return await _context.InterviewFeedbacks
          .Include(f => f.Skill)
          .Include(f => f.FeedbackByEmployee)
          .Where(f => f.Id == id)
          .Select(f => new InterviewFeedbackDto
          {
            Id = f.Id,
            InterviewId = f.InterviewId,
            ForSkill = f.ForSkill,
            SkillName = f.Skill.SkillName,
            FeedbackBy = f.FeedbackBy,
            FeedbackByName = f.FeedbackByEmployee.FullName ?? "Unknown",
            Rating = f.Rating,
            Feedback = f.Feedback,
            CreatedAt = f.CreatedAt
          })
          .FirstOrDefaultAsync();
    }

    public async Task<InterviewFeedbackDto> CreateFeedbackAsync(CreateInterviewFeedbackDto dto, Guid feedbackBy)
    {
      var feedback = new InterviewFeedback
      {
        Id = Guid.NewGuid(),
        InterviewId = dto.InterviewId,
        ForSkill = dto.ForSkill,
        FeedbackBy = feedbackBy,
        Rating = dto.Rating,
        Feedback = dto.Feedback,
        CreatedAt = DateTime.UtcNow
      };

      _context.InterviewFeedbacks.Add(feedback);
      await _context.SaveChangesAsync();
      return (await GetFeedbackByIdAsync(feedback.Id))!;
    }

    public async Task<InterviewFeedbackDto?> UpdateFeedbackAsync(Guid id, UpdateInterviewFeedbackDto dto)
    {
      var feedback = await _context.InterviewFeedbacks.FindAsync(id);
      if (feedback == null) return null;

      if (dto.Rating.HasValue)
        feedback.Rating = dto.Rating.Value;
      if (dto.Feedback != null)
        feedback.Feedback = dto.Feedback;

      await _context.SaveChangesAsync();
      return await GetFeedbackByIdAsync(id);
    }

    public async Task<bool> DeleteFeedbackAsync(Guid id)
    {
      var feedback = await _context.InterviewFeedbacks.FindAsync(id);
      if (feedback == null) return false;

      _context.InterviewFeedbacks.Remove(feedback);
      await _context.SaveChangesAsync();
      return true;
    }


    public async Task<IEnumerable<InterviewType>> GetInterviewTypesAsync()
    {
      return await _context.InterviewTypes.ToListAsync();
    }

  }
}
