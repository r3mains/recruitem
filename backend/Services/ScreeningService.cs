using AutoMapper;
using Backend.Data;
using Backend.Dtos.JobApplications;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ScreeningService(AppDbContext context, IMapper mapper) : IScreeningService
{
  public async Task<JobApplicationDto?> ScreenApplicationAsync(Guid applicationId, ScreeningDto screeningDto, Guid reviewerId)
  {
    var application = await context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.Recruiter)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.User)
        .Include(ja => ja.Status)
        .FirstOrDefaultAsync(ja => ja.Id == applicationId);

    if (application == null) return null;

    application.StatusId = screeningDto.StatusId;
    application.Notes = screeningDto.Notes;
    application.Score = screeningDto.Score;
    application.ReviewedAt = DateTime.UtcNow;
    application.ReviewedBy = reviewerId;
    application.LastUpdated = DateTime.UtcNow;

    await context.SaveChangesAsync();
    return mapper.Map<JobApplicationDto>(application);
  }

  public async Task<List<JobApplicationDto>> BulkScreenApplicationsAsync(BulkScreeningDto bulkScreeningDto, Guid reviewerId)
  {
    var applications = await context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.Recruiter)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.User)
        .Include(ja => ja.Status)
        .Where(ja => bulkScreeningDto.ApplicationIds.Contains(ja.Id))
        .ToListAsync();

    foreach (var application in applications)
    {
      application.StatusId = bulkScreeningDto.StatusId;
      application.Notes = bulkScreeningDto.Notes;
      
      if (bulkScreeningDto.MinScore.HasValue && bulkScreeningDto.MaxScore.HasValue)
      {
        var calculatedScore = await CalculateApplicationScoreAsync(application.Id);
        if (calculatedScore >= bulkScreeningDto.MinScore && calculatedScore <= bulkScreeningDto.MaxScore)
        {
          application.Score = calculatedScore;
        }
      }
      
      application.ReviewedAt = DateTime.UtcNow;
      application.ReviewedBy = reviewerId;
      application.LastUpdated = DateTime.UtcNow;
    }

    await context.SaveChangesAsync();
    return mapper.Map<List<JobApplicationDto>>(applications);
  }

  public async Task<List<JobApplicationDto>> ShortlistApplicationsAsync(ShortlistingDto shortlistingDto, Guid reviewerId)
  {
    var applications = await context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.Recruiter)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.User)
        .Include(ja => ja.Status)
        .Where(ja => shortlistingDto.ApplicationIds.Contains(ja.Id))
        .ToListAsync();

    foreach (var application in applications)
    {
      application.StatusId = shortlistingDto.StatusId;
      application.Notes = shortlistingDto.Notes;
      application.ReviewedAt = DateTime.UtcNow;
      application.ReviewedBy = reviewerId;
      application.LastUpdated = DateTime.UtcNow;
    }

    await context.SaveChangesAsync();
    return mapper.Map<List<JobApplicationDto>>(applications);
  }

  public async Task<List<JobApplicationDto>> GetApplicationsForScreeningAsync(Guid? jobId = null, Guid? statusId = null)
  {
    var query = context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.Recruiter)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.User)
        .Include(ja => ja.Status)
        .AsQueryable();

    if (jobId.HasValue)
    {
      query = query.Where(ja => ja.JobId == jobId);
    }

    if (statusId.HasValue)
    {
      query = query.Where(ja => ja.StatusId == statusId);
    }
    else
    {
      var pendingStatuses = await context.StatusTypes
          .Where(st => st.Context == "application" && 
                      (st.Status == "Applied" || st.Status == "Under Review"))
          .Select(st => st.Id)
          .ToListAsync();
      
      query = query.Where(ja => pendingStatuses.Contains(ja.StatusId));
    }

    var applications = await query
        .OrderBy(ja => ja.AppliedAt)
        .ToListAsync();

    return mapper.Map<List<JobApplicationDto>>(applications);
  }

  public async Task<List<JobApplicationDto>> GetShortlistedApplicationsAsync(Guid? jobId = null)
  {
    var shortlistedStatus = await context.StatusTypes
        .FirstOrDefaultAsync(st => st.Context == "application" && st.Status == "Shortlisted");

    if (shortlistedStatus == null) return [];

    var query = context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.Recruiter)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.User)
        .Include(ja => ja.Status)
        .Where(ja => ja.StatusId == shortlistedStatus.Id);

    if (jobId.HasValue)
    {
      query = query.Where(ja => ja.JobId == jobId);
    }

    var applications = await query
        .OrderByDescending(ja => ja.Score)
        .ThenBy(ja => ja.AppliedAt)
        .ToListAsync();

    return mapper.Map<List<JobApplicationDto>>(applications);
  }

  public async Task<double> CalculateApplicationScoreAsync(Guid applicationId)
  {
    var application = await context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.JobSkills)
                .ThenInclude(js => js.Skill)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.CandidateQualifications)
        .FirstOrDefaultAsync(ja => ja.Id == applicationId);

    if (application?.Job == null || application.Candidate == null)
        return 0.0;

    double score = 0.0;
    int totalCriteria = 0;

    var jobSkills = application.Job.JobSkills;
    var candidateSkills = application.Candidate.CandidateSkills;

    foreach (var jobSkill in jobSkills)
    {
      var candidateSkill = candidateSkills
          .FirstOrDefault(cs => cs.SkillId == jobSkill.SkillId);

      if (candidateSkill != null)
      {
        double skillScore = jobSkill.IsRequired ? 20.0 : 10.0;
        
        if (candidateSkill.YearsOfExperience >= 3)
          skillScore += 10.0;
        else if (candidateSkill.YearsOfExperience >= 1)
          skillScore += 5.0;

        score += skillScore;
      }
      else if (jobSkill.IsRequired)
      {
        score -= 15.0;
      }

      totalCriteria++;
    }

    if (application.Candidate.CandidateQualifications.Any())
    {
      score += 15.0;
    }

    if (!string.IsNullOrEmpty(application.CoverLetterUrl))
    {
      score += 5.0;
    }

    if (!string.IsNullOrEmpty(application.Candidate.ResumeUrl))
    {
      score += 5.0;
    }

    return Math.Max(0, Math.Min(100, score));
  }

  public async Task<List<JobApplicationDto>> GetApplicationsByScoreRangeAsync(Guid jobId, double minScore, double maxScore)
  {
    var applications = await context.JobApplications
        .Include(ja => ja.Job)
            .ThenInclude(j => j!.Recruiter)
        .Include(ja => ja.Candidate)
            .ThenInclude(c => c!.User)
        .Include(ja => ja.Status)
        .Where(ja => ja.JobId == jobId && 
                    ja.Score.HasValue && 
                    ja.Score >= minScore && 
                    ja.Score <= maxScore)
        .OrderByDescending(ja => ja.Score)
        .ToListAsync();

    return mapper.Map<List<JobApplicationDto>>(applications);
  }
}
