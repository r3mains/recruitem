using backend.DTOs.Reports;

namespace backend.Repositories.IRepositories;

public interface IReportsRepository
{
  Task<DashboardStatsDto> GetDashboardStatsAsync();
  Task<RecruitmentPipelineDto> GetRecruitmentPipelineAsync();
  Task<IEnumerable<JobStatsDto>> GetJobStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
  Task<IEnumerable<RecruiterPerformanceDto>> GetRecruiterPerformanceAsync(DateTime? startDate = null, DateTime? endDate = null);
  Task<TimeToHireDto> GetTimeToHireMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);
  Task<IEnumerable<StatusDistributionDto>> GetStatusDistributionAsync();
  Task<InterviewStatsDto> GetInterviewStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
  Task<IEnumerable<SourceAnalysisDto>> GetSourceAnalysisAsync();
  Task<IEnumerable<MonthlyTrendsDto>> GetMonthlyTrendsAsync(int months = 12);
  Task<IEnumerable<SkillDemandDto>> GetSkillDemandAsync();
  Task<IEnumerable<ApplicationFunnelDto>> GetApplicationFunnelAsync(Guid? jobId = null);
  Task<IEnumerable<ExperienceWiseCandidateDto>> GetExperienceWiseCandidatesAsync();
  Task<IEnumerable<CollegeWiseReportDto>> GetCollegeWiseReportAsync();
}
