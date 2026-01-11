namespace backend.DTOs.Reports;

// Dashboard Statistics
public record DashboardStatsDto(
  int TotalJobs,
  int ActiveJobs,
  int ClosedJobs,
  int TotalCandidates,
  int TotalApplications,
  int PendingApplications,
  int ShortlistedApplications,
  int InterviewsScheduled,
  int InterviewsCompleted,
  int OffersExtended,
  int CandidatesHired
);

// Recruitment Pipeline
public record RecruitmentPipelineDto(
  int NewApplications,
  int UnderReview,
  int Shortlisted,
  int InterviewScheduled,
  int InterviewCompleted,
  int OfferExtended,
  int Hired,
  int Rejected
);

// Job Statistics
public record JobStatsDto(
  Guid JobId,
  string JobTitle,
  string Status,
  int TotalApplications,
  int NewApplications,
  int UnderReview,
  int Shortlisted,
  int InterviewScheduled,
  int Hired,
  int Rejected,
  DateTime CreatedAt,
  DateTime? ClosedAt
);

// Recruiter Performance
public record RecruiterPerformanceDto(
  Guid RecruiterId,
  string RecruiterName,
  int JobsPosted,
  int ActiveJobs,
  int TotalApplications,
  int CandidatesShortlisted,
  int CandidatesHired,
  decimal AverageTimeToHire,
  decimal HireConversionRate
);

// Time to Hire Metrics
public record TimeToHireDto(
  decimal AverageTimeToHireDays,
  decimal MedianTimeToHireDays,
  decimal MinTimeToHireDays,
  decimal MaxTimeToHireDays,
  int TotalHires
);

// Application Status Distribution
public record StatusDistributionDto(
  string Status,
  int Count,
  decimal Percentage
);

// Interview Statistics
public record InterviewStatsDto(
  int TotalInterviewsScheduled,
  int InterviewsCompleted,
  int InterviewsPending,
  int InterviewsCancelled,
  decimal AverageInterviewDuration,
  decimal PassRate
);

// Source Analysis
public record SourceAnalysisDto(
  string Source,
  int Applications,
  int Hires,
  decimal ConversionRate
);

// Monthly Trends
public record MonthlyTrendsDto(
  int Year,
  int Month,
  string MonthName,
  int JobsPosted,
  int ApplicationsReceived,
  int CandidatesHired
);

// Skill Demand
public record SkillDemandDto(
  Guid SkillId,
  string SkillName,
  int JobPostings,
  int CandidatesWithSkill,
  int Demand
);

// Application funnel
public record ApplicationFunnelDto(
  string Stage,
  int Count,
  decimal DropOffRate,
  decimal ConversionRate
);
