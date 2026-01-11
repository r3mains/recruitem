using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Services.IServices;
using System.Text.Json;

namespace backend.Services;

public class ScoringService(ApplicationDbContext context, ILogger<ScoringService> logger) : IScoringService
{
  private readonly ApplicationDbContext _context = context;
  private readonly ILogger<ScoringService> _logger = logger;

  public async Task<AutomatedScore> CalculateScoreAsync(Guid jobApplicationId)
  {
    var application = await _context.JobApplications
      .Include(ja => ja.Job)
        .ThenInclude(j => j.Position)
          .ThenInclude(p => p.PositionSkills)
      .Include(ja => ja.Job)
        .ThenInclude(j => j.JobSkills)
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.CandidateSkills)
      .Include(ja => ja.Candidate)
        .ThenInclude(c => c.CandidateQualifications)
      .Include(ja => ja.Interviews)
        .ThenInclude(i => i.InterviewFeedbacks)
      .FirstOrDefaultAsync(ja => ja.Id == jobApplicationId && !ja.IsDeleted)
      ?? throw new InvalidOperationException("Job application not found");

    var positionId = application.Job.PositionId;
    var config = await GetScoringConfigurationAsync(positionId) 
                 ?? await GetDefaultScoringConfigurationAsync(positionId);

    // Calculate individual scores
    var skillScore = CalculateSkillMatchScore(application);
    var experienceScore = CalculateExperienceScore(application);
    var interviewScore = CalculateInterviewScore(application);
    var educationScore = CalculateEducationScore(application);

    // Calculate weighted total (testScore removed as OnlineTest feature is removed)
    var totalScore = 
      (skillScore * config.SkillMatchWeight / 100) +
      (experienceScore * config.ExperienceWeight / 100) +
      (interviewScore * config.InterviewWeight / 100) +
      (educationScore * config.EducationWeight / 100);

    // Create score breakdown
    var breakdown = new
    {
      SkillMatch = new { Score = skillScore, Weight = config.SkillMatchWeight },
      Experience = new { Score = experienceScore, Weight = config.ExperienceWeight },
      Interview = new { Score = interviewScore, Weight = config.InterviewWeight },
      Education = new { Score = educationScore, Weight = config.EducationWeight },
      Total = totalScore
    };

    // Check if score already exists
    var existingScore = await _context.AutomatedScores
      .FirstOrDefaultAsync(s => s.JobApplicationId == jobApplicationId && !s.IsDeleted);

    if (existingScore != null)
    {
      existingScore.SkillMatchScore = skillScore;
      existingScore.ExperienceScore = experienceScore;
      existingScore.InterviewScore = interviewScore;
      existingScore.TestScore = 0;
      existingScore.EducationScore = educationScore;
      existingScore.TotalWeightedScore = totalScore;
      existingScore.ScoreBreakdown = JsonSerializer.Serialize(breakdown);
      existingScore.CalculatedAt = DateTime.UtcNow;
      
      _context.AutomatedScores.Update(existingScore);
    }
    else
    {
      existingScore = new AutomatedScore
      {
        JobApplicationId = jobApplicationId,
        SkillMatchScore = skillScore,
        ExperienceScore = experienceScore,
        InterviewScore = interviewScore,
        TestScore = 0,
        EducationScore = educationScore,
        TotalWeightedScore = totalScore,
        ScoreBreakdown = JsonSerializer.Serialize(breakdown),
        CalculatedAt = DateTime.UtcNow
      };
      
      _context.AutomatedScores.Add(existingScore);
    }

    await _context.SaveChangesAsync();
    return existingScore;
  }

  private decimal CalculateSkillMatchScore(JobApplication application)
  {
    var requiredSkills = application.Job.JobSkills
      .Where(js => js.Required)
      .Select(js => js.SkillId)
      .ToList();

    var preferredSkills = application.Job.JobSkills
      .Where(js => !js.Required)
      .Select(js => js.SkillId)
      .ToList();

    var candidateSkills = application.Candidate.CandidateSkills
      .Select(cs => cs.SkillId)
      .ToList();

    if (!requiredSkills.Any() && !preferredSkills.Any())
      return 0;

    // Calculate required skills match
    var matchedRequired = requiredSkills.Count(rs => candidateSkills.Contains(rs));
    var requiredScore = requiredSkills.Any() 
      ? (decimal)matchedRequired / requiredSkills.Count() * 70 
      : 70;

    // Calculate preferred skills match
    var matchedPreferred = preferredSkills.Count(ps => candidateSkills.Contains(ps));
    var preferredScore = preferredSkills.Any() 
      ? (decimal)matchedPreferred / preferredSkills.Count() * 30 
      : 30;

    return Math.Min(100, requiredScore + preferredScore);
  }

  private decimal CalculateExperienceScore(JobApplication application)
  {
    // Calculate based on total years of experience across all skills
    var totalExperience = application.Candidate.CandidateSkills
      .Sum(cs => cs.YearOfExperience ?? 0);

    // Normalize to 0-100 scale (assuming 10+ years is maximum)
    var score = Math.Min(100, (decimal)totalExperience / 10 * 100);
    return score;
  }

  private decimal CalculateInterviewScore(JobApplication application)
  {
    var feedbacks = application.Interviews
      .SelectMany(i => i.InterviewFeedbacks)
      .Where(f => f.Rating > 0)
      .ToList();

    if (!feedbacks.Any())
      return 0;

    // Average all ratings and normalize to 0-100
    var averageRating = feedbacks.Average(f => f.Rating);
    return (decimal)averageRating / 5 * 100;
  }

  private decimal CalculateEducationScore(JobApplication application)
  {
    var qualifications = application.Candidate.CandidateQualifications.Count;
    
    // Simple scoring: more qualifications = higher score
    // Cap at 100
    return Math.Min(100, qualifications * 25);
  }

  public async Task<AutomatedScore?> GetScoreAsync(Guid jobApplicationId)
  {
    return await _context.AutomatedScores
      .Include(s => s.JobApplication)
      .FirstOrDefaultAsync(s => s.JobApplicationId == jobApplicationId && !s.IsDeleted);
  }

  public async Task<IEnumerable<AutomatedScore>> GetScoresByJobAsync(Guid jobId)
  {
    return await _context.AutomatedScores
      .Include(s => s.JobApplication)
      .Where(s => s.JobApplication.JobId == jobId && !s.IsDeleted)
      .OrderByDescending(s => s.TotalWeightedScore)
      .ToListAsync();
  }

  public async Task<ScoringConfiguration?> GetScoringConfigurationAsync(Guid positionId)
  {
    return await _context.ScoringConfigurations
      .FirstOrDefaultAsync(sc => sc.PositionId == positionId && sc.IsActive);
  }

  public async Task<ScoringConfiguration> CreateOrUpdateScoringConfigurationAsync(ScoringConfiguration config)
  {
    var existing = await _context.ScoringConfigurations
      .FirstOrDefaultAsync(sc => sc.PositionId == config.PositionId && sc.IsActive);

    if (existing != null)
    {
      existing.SkillMatchWeight = config.SkillMatchWeight;
      existing.ExperienceWeight = config.ExperienceWeight;
      existing.InterviewWeight = config.InterviewWeight;
      existing.TestWeight = config.TestWeight;
      existing.EducationWeight = config.EducationWeight;
      existing.UpdatedAt = DateTime.UtcNow;
      
      _context.ScoringConfigurations.Update(existing);
      await _context.SaveChangesAsync();
      return existing;
    }

    _context.ScoringConfigurations.Add(config);
    await _context.SaveChangesAsync();
    return config;
  }

  private async Task<ScoringConfiguration> GetDefaultScoringConfigurationAsync(Guid positionId)
  {
    var defaultConfig = new ScoringConfiguration
    {
      PositionId = positionId,
      SkillMatchWeight = 30.0m,
      ExperienceWeight = 20.0m,
      InterviewWeight = 30.0m,
      TestWeight = 15.0m,
      EducationWeight = 5.0m,
      IsActive = true
    };

    _context.ScoringConfigurations.Add(defaultConfig);
    await _context.SaveChangesAsync();
    return defaultConfig;
  }
}
