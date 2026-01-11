using backend.Data;
using backend.DTOs.Reports;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace backend.Repositories;

public class ReportsRepository(ApplicationDbContext context) : IReportsRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<DashboardStatsDto> GetDashboardStatsAsync()
  {
    var totalJobs = await _context.Jobs.CountAsync(j => !j.IsDeleted);
    var activeJobs = await _context.Jobs.CountAsync(j => !j.IsDeleted && j.Status.Status == "Open");
    var closedJobs = await _context.Jobs.CountAsync(j => !j.IsDeleted && j.Status.Status == "Closed");
    
    var totalCandidates = await _context.Candidates.CountAsync();
    var totalApplications = await _context.JobApplications.CountAsync(ja => !ja.IsDeleted);
    
    var pendingApplications = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && (ja.Status.Status == "Applied" || ja.Status.Status == "Under Review"));
    
    var shortlistedApplications = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Shortlisted");
    
    var interviewsScheduled = await _context.Interviews
      .CountAsync(i => i.Status != null && i.Status.Status == "Scheduled");
    
    var interviewsCompleted = await _context.Interviews
      .CountAsync(i => i.Status != null && i.Status.Status == "Completed");
    
    var offersExtended = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Offer Extended");
    
    var candidatesHired = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Hired");

    return new DashboardStatsDto(
      totalJobs,
      activeJobs,
      closedJobs,
      totalCandidates,
      totalApplications,
      pendingApplications,
      shortlistedApplications,
      interviewsScheduled,
      interviewsCompleted,
      offersExtended,
      candidatesHired
    );
  }

  public async Task<RecruitmentPipelineDto> GetRecruitmentPipelineAsync()
  {
    var newApplications = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Applied");
    
    var underReview = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Under Review");
    
    var shortlisted = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Shortlisted");
    
    var interviewScheduled = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Interview Scheduled");
    
    var interviewCompleted = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Interview Completed");
    
    var offerExtended = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Offer Extended");
    
    var hired = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Hired");
    
    var rejected = await _context.JobApplications
      .CountAsync(ja => !ja.IsDeleted && ja.Status.Status == "Rejected");

    return new RecruitmentPipelineDto(
      newApplications,
      underReview,
      shortlisted,
      interviewScheduled,
      interviewCompleted,
      offerExtended,
      hired,
      rejected
    );
  }

  public async Task<IEnumerable<JobStatsDto>> GetJobStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
  {
    var query = _context.Jobs
      .Include(j => j.Status)
      .Include(j => j.JobApplications)
        .ThenInclude(ja => ja.Status)
      .Where(j => !j.IsDeleted);

    if (startDate.HasValue)
      query = query.Where(j => j.CreatedAt >= startDate.Value);
    
    if (endDate.HasValue)
      query = query.Where(j => j.CreatedAt <= endDate.Value);

    var jobs = await query.ToListAsync();

    return jobs.Select(j => new JobStatsDto(
      j.Id,
      j.Title,
      j.Status.Status,
      j.JobApplications.Count(ja => !ja.IsDeleted),
      j.JobApplications.Count(ja => !ja.IsDeleted && ja.Status.Status == "Applied"),
      j.JobApplications.Count(ja => !ja.IsDeleted && ja.Status.Status == "Under Review"),
      j.JobApplications.Count(ja => !ja.IsDeleted && ja.Status.Status == "Shortlisted"),
      j.JobApplications.Count(ja => !ja.IsDeleted && ja.Status.Status == "Interview Scheduled"),
      j.JobApplications.Count(ja => !ja.IsDeleted && ja.Status.Status == "Hired"),
      j.JobApplications.Count(ja => !ja.IsDeleted && ja.Status.Status == "Rejected"),
      j.CreatedAt,
      null
    ));
  }

  public async Task<IEnumerable<RecruiterPerformanceDto>> GetRecruiterPerformanceAsync(DateTime? startDate = null, DateTime? endDate = null)
  {
    var query = _context.Employees
      .Include(e => e.RecruitedJobs)
        .ThenInclude(j => j.Status)
      .Include(e => e.RecruitedJobs)
        .ThenInclude(j => j.JobApplications)
          .ThenInclude(ja => ja.Status)
      .Where(e => e.RecruitedJobs.Any());

    var recruiters = await query.ToListAsync();

    return recruiters.Select(r =>
    {
      var jobs = r.RecruitedJobs.Where(j => !j.IsDeleted);
      if (startDate.HasValue)
        jobs = jobs.Where(j => j.CreatedAt >= startDate.Value);
      if (endDate.HasValue)
        jobs = jobs.Where(j => j.CreatedAt <= endDate.Value);

      var jobsList = jobs.ToList();
      var allApplications = jobsList.SelectMany(j => j.JobApplications.Where(ja => !ja.IsDeleted)).ToList();
      var hiredCandidates = allApplications.Where(ja => ja.Status?.Status == "Hired").ToList();
      
      var avgTimeToHire = hiredCandidates.Any() && hiredCandidates.All(ja => ja.AppliedAt.HasValue && ja.LastUpdated.HasValue)
        ? (decimal)hiredCandidates.Average(ja => (ja.LastUpdated!.Value - ja.AppliedAt!.Value).TotalDays)
        : 0;

      var hireRate = allApplications.Any() 
        ? (decimal)hiredCandidates.Count / allApplications.Count * 100
        : 0;

      return new RecruiterPerformanceDto(
        r.Id,
        r.FullName ?? "",
        jobsList.Count,
        jobsList.Count(j => j.Status?.Status == "Open"),
        allApplications.Count,
        allApplications.Count(ja => ja.Status?.Status == "Shortlisted"),
        hiredCandidates.Count,
        avgTimeToHire,
        hireRate
      );
    });
  }

  public async Task<TimeToHireDto> GetTimeToHireMetricsAsync(DateTime? startDate = null, DateTime? endDate = null)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted && 
                   ja.Status.Status == "Hired" && 
                   ja.AppliedAt.HasValue && 
                   ja.LastUpdated.HasValue);

    if (startDate.HasValue)
      query = query.Where(ja => ja.LastUpdated >= startDate.Value);
    
    if (endDate.HasValue)
      query = query.Where(ja => ja.LastUpdated <= endDate.Value);

    var hiredApplications = await query.ToListAsync();

    if (!hiredApplications.Any())
    {
      return new TimeToHireDto(0, 0, 0, 0, 0);
    }

    var timeToHireDays = hiredApplications
      .Select(ja => (decimal)(ja.LastUpdated!.Value - ja.AppliedAt!.Value).TotalDays)
      .OrderBy(d => d)
      .ToList();

    var average = timeToHireDays.Average();
    var median = timeToHireDays.Count % 2 == 0
      ? (timeToHireDays[timeToHireDays.Count / 2 - 1] + timeToHireDays[timeToHireDays.Count / 2]) / 2
      : timeToHireDays[timeToHireDays.Count / 2];

    return new TimeToHireDto(
      average,
      median,
      timeToHireDays.Min(),
      timeToHireDays.Max(),
      hiredApplications.Count
    );
  }

  public async Task<IEnumerable<StatusDistributionDto>> GetStatusDistributionAsync()
  {
    var total = await _context.JobApplications.CountAsync(ja => !ja.IsDeleted);
    
    if (total == 0)
      return Enumerable.Empty<StatusDistributionDto>();

    var distribution = await _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted)
      .GroupBy(ja => ja.Status.Status)
      .Select(g => new
      {
        Status = g.Key,
        Count = g.Count()
      })
      .ToListAsync();

    return distribution.Select(d => new StatusDistributionDto(
      d.Status,
      d.Count,
      (decimal)d.Count / total * 100
    ));
  }

  public async Task<InterviewStatsDto> GetInterviewStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
  {
    var query = _context.Interviews
      .Include(i => i.Status)
      .Include(i => i.InterviewSchedules)
      .AsQueryable();

    if (startDate.HasValue)
      query = query.Where(i => i.CreatedAt >= startDate.Value);
    
    if (endDate.HasValue)
      query = query.Where(i => i.CreatedAt <= endDate.Value);

    var interviews = await query.ToListAsync();

    var totalScheduled = interviews.Count;
    var completed = interviews.Count(i => i.Status != null && i.Status.Status == "Completed");
    var pending = interviews.Count(i => i.Status != null && i.Status.Status == "Scheduled");
    var cancelled = interviews.Count(i => i.Status != null && i.Status.Status == "Cancelled");

    // Average duration not available in current model - set to 0
    var avgDuration = 0m;

    var feedbackCount = await _context.InterviewFeedbacks.CountAsync();
    var passRate = completed > 0 && feedbackCount > 0
      ? (decimal)feedbackCount / completed * 100
      : 0;

    return new InterviewStatsDto(
      totalScheduled,
      completed,
      pending,
      cancelled,
      avgDuration,
      passRate
    );
  }

  public async Task<IEnumerable<SourceAnalysisDto>> GetSourceAnalysisAsync()
  {
    return await Task.FromResult(Enumerable.Empty<SourceAnalysisDto>());
  }

  public async Task<IEnumerable<MonthlyTrendsDto>> GetMonthlyTrendsAsync(int months = 12)
  {
    var startDate = DateTime.UtcNow.AddMonths(-months);

    var jobsByMonth = await _context.Jobs
      .Where(j => !j.IsDeleted && j.CreatedAt >= startDate)
      .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
      .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
      .ToListAsync();

    var applicationsByMonth = await _context.JobApplications
      .Where(ja => !ja.IsDeleted && ja.AppliedAt.HasValue && ja.AppliedAt >= startDate)
      .GroupBy(ja => new { ja.AppliedAt!.Value.Year, ja.AppliedAt.Value.Month })
      .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
      .ToListAsync();

    var hiresByMonth = await _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted && ja.Status.Status == "Hired" && ja.LastUpdated.HasValue && ja.LastUpdated >= startDate)
      .GroupBy(ja => new { ja.LastUpdated!.Value.Year, ja.LastUpdated.Value.Month })
      .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
      .ToListAsync();

    var trends = new List<MonthlyTrendsDto>();
    for (int i = 0; i < months; i++)
    {
      var date = DateTime.UtcNow.AddMonths(-i);
      var year = date.Year;
      var month = date.Month;

      trends.Add(new MonthlyTrendsDto(
        year,
        month,
        CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
        jobsByMonth.FirstOrDefault(j => j.Year == year && j.Month == month)?.Count ?? 0,
        applicationsByMonth.FirstOrDefault(a => a.Year == year && a.Month == month)?.Count ?? 0,
        hiresByMonth.FirstOrDefault(h => h.Year == year && h.Month == month)?.Count ?? 0
      ));
    }

    return trends.OrderBy(t => t.Year).ThenBy(t => t.Month);
  }

  public async Task<IEnumerable<SkillDemandDto>> GetSkillDemandAsync()
  {
    var skills = await _context.Skills
      .Include(s => s.JobSkills)
      .Include(s => s.CandidateSkills)
      .ToListAsync();

    return skills.Select(s => new SkillDemandDto(
      s.Id,
      s.SkillName,
      s.JobSkills.Count,
      s.CandidateSkills.Count,
      Math.Max(0, s.JobSkills.Count - s.CandidateSkills.Count)
    ))
    .OrderByDescending(s => s.Demand);
  }

  public async Task<IEnumerable<ApplicationFunnelDto>> GetApplicationFunnelAsync(Guid? jobId = null)
  {
    var query = _context.JobApplications
      .Include(ja => ja.Status)
      .Where(ja => !ja.IsDeleted);

    if (jobId.HasValue)
      query = query.Where(ja => ja.JobId == jobId.Value);

    var applications = await query.ToListAsync();
    var total = applications.Count;

    if (total == 0)
      return Enumerable.Empty<ApplicationFunnelDto>();

    var stages = new[]
    {
      "Applied",
      "Under Review",
      "Shortlisted",
      "Interview Scheduled",
      "Interview Completed",
      "Offer Extended",
      "Hired"
    };

    var funnel = new List<ApplicationFunnelDto>();
    int previousCount = total;

    foreach (var stage in stages)
    {
      var count = applications.Count(a => a.Status.Status == stage);
      var dropOffRate = previousCount > 0 ? (decimal)(previousCount - count) / previousCount * 100 : 0;
      var conversionRate = total > 0 ? (decimal)count / total * 100 : 0;

      funnel.Add(new ApplicationFunnelDto(
        stage,
        count,
        dropOffRate,
        conversionRate
      ));

      previousCount = count;
    }

    return funnel;
  }

  public async Task<IEnumerable<ExperienceWiseCandidateDto>> GetExperienceWiseCandidatesAsync()
  {
    var candidates = await _context.Candidates
      .Include(c => c.User)
      .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
      .Where(c => !c.IsDeleted)
      .ToListAsync();

    // Group candidates by experience ranges
    var experienceRanges = new[]
    {
      new { Min = 0, Max = 1, Label = "0-1 years" },
      new { Min = 1, Max = 3, Label = "1-3 years" },
      new { Min = 3, Max = 5, Label = "3-5 years" },
      new { Min = 5, Max = 8, Label = "5-8 years" },
      new { Min = 8, Max = 12, Label = "8-12 years" },
      new { Min = 12, Max = int.MaxValue, Label = "12+ years" }
    };

    var result = new List<ExperienceWiseCandidateDto>();

    foreach (var range in experienceRanges)
    {
      var candidatesInRange = candidates
        .Select(c => new
        {
          Candidate = c,
          TotalExperience = c.CandidateSkills.Sum(cs => cs.YearOfExperience ?? 0)
        })
        .Where(x => x.TotalExperience >= range.Min && x.TotalExperience < range.Max)
        .ToList();

      if (candidatesInRange.Any())
      {
        result.Add(new ExperienceWiseCandidateDto
        {
          ExperienceRange = range.Label,
          MinYears = range.Min,
          MaxYears = range.Max == int.MaxValue ? 100 : range.Max,
          CandidateCount = candidatesInRange.Count,
          Candidates = candidatesInRange.Select(x => new CandidateSummary
          {
            Id = x.Candidate.Id,
            FullName = x.Candidate.FullName,
            Email = x.Candidate.User?.Email,
            TotalExperience = x.TotalExperience,
            Skills = x.Candidate.CandidateSkills.Select(cs => cs.Skill.SkillName).ToList()
          }).ToList()
        });
      }
    }

    return result;
  }

  public async Task<IEnumerable<CollegeWiseReportDto>> GetCollegeWiseReportAsync()
  {
    var candidates = await _context.Candidates
      .Include(c => c.JobApplications)
        .ThenInclude(ja => ja.Status)
      .Where(c => !c.IsDeleted && !string.IsNullOrEmpty(c.College))
      .ToListAsync();

    var collegeGroups = candidates
      .GroupBy(c => c.College)
      .Select(g => new
      {
        College = g.Key!,
        Candidates = g.ToList()
      })
      .ToList();

    var result = new List<CollegeWiseReportDto>();

    foreach (var group in collegeGroups)
    {
      var allApplications = group.Candidates
        .SelectMany(c => c.JobApplications)
        .Where(ja => !ja.IsDeleted)
        .ToList();

      var hiredApplications = allApplications
        .Where(ja => ja.Status.Status == "Hired")
        .ToList();

      var shortlistedCount = allApplications.Count(ja => ja.Status.Status == "Shortlisted");
      var interviewedCount = allApplications.Count(ja => 
        ja.Status.Status == "Interview Scheduled" || 
        ja.Status.Status == "Interview Completed" ||
        ja.Status.Status == "Offer Extended" ||
        ja.Status.Status == "Hired");

      // Calculate average time to hire
      double? avgTimeToHire = null;
      if (hiredApplications.Any())
      {
        var timeToHires = hiredApplications
          .Where(ja => ja.AppliedAt.HasValue && ja.LastUpdated.HasValue)
          .Select(ja => (ja.LastUpdated!.Value - ja.AppliedAt!.Value).TotalDays)
          .ToList();

        if (timeToHires.Any())
          avgTimeToHire = timeToHires.Average();
      }

      // Year-wise stats
      var yearStats = group.Candidates
        .Where(c => c.GraduationYear.HasValue)
        .GroupBy(c => c.GraduationYear!.Value)
        .Select(yg => new CollegeGraduationYearStats
        {
          Year = yg.Key,
          CandidateCount = yg.Count(),
          HiredCount = yg.Count(c => c.JobApplications.Any(ja => !ja.IsDeleted && ja.Status.Status == "Hired"))
        })
        .OrderByDescending(y => y.Year)
        .ToList();

      result.Add(new CollegeWiseReportDto
      {
        College = group.College,
        TotalCandidates = group.Candidates.Count,
        TotalApplications = allApplications.Count,
        ShortlistedCount = shortlistedCount,
        InterviewedCount = interviewedCount,
        HiredCount = hiredApplications.Count,
        SuccessRate = allApplications.Count > 0 
          ? Math.Round((decimal)hiredApplications.Count / allApplications.Count * 100, 2)
          : 0,
        AverageTimeToHireDays = avgTimeToHire.HasValue ? Math.Round(avgTimeToHire.Value, 2) : null,
        YearWiseStats = yearStats.Any() ? yearStats : null
      });
    }

    return result.OrderByDescending(r => r.TotalCandidates);
  }
}
