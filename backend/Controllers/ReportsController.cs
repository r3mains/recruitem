using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("reports")]
[Authorize(Policy = "ViewReports")]
public class ReportsController(IReportsRepository reportsRepository) : ControllerBase
{
  private readonly IReportsRepository _reportsRepository = reportsRepository;

  [HttpGet("dashboard")]
  [Authorize(Policy = "ViewDashboard")]
  public async Task<IActionResult> GetDashboardStats()
  {
    try
    {
      var stats = await _reportsRepository.GetDashboardStatsAsync();
      return Ok(stats);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics.", error = ex.Message });
    }
  }

  [HttpGet("pipeline")]
  public async Task<IActionResult> GetRecruitmentPipeline()
  {
    try
    {
      var pipeline = await _reportsRepository.GetRecruitmentPipelineAsync();
      return Ok(pipeline);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving pipeline data.", error = ex.Message });
    }
  }

  [HttpGet("job-stats")]
  public async Task<IActionResult> GetJobStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
  {
    try
    {
      var stats = await _reportsRepository.GetJobStatsAsync(startDate, endDate);
      return Ok(stats);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving job statistics.", error = ex.Message });
    }
  }

  [HttpGet("recruiter-performance")]
  public async Task<IActionResult> GetRecruiterPerformance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
  {
    try
    {
      var performance = await _reportsRepository.GetRecruiterPerformanceAsync(startDate, endDate);
      return Ok(performance);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving recruiter performance.", error = ex.Message });
    }
  }

  [HttpGet("time-to-hire")]
  public async Task<IActionResult> GetTimeToHireMetrics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
  {
    try
    {
      var metrics = await _reportsRepository.GetTimeToHireMetricsAsync(startDate, endDate);
      return Ok(metrics);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving time-to-hire metrics.", error = ex.Message });
    }
  }

  [HttpGet("status-distribution")]
  public async Task<IActionResult> GetStatusDistribution()
  {
    try
    {
      var distribution = await _reportsRepository.GetStatusDistributionAsync();
      return Ok(distribution);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving status distribution.", error = ex.Message });
    }
  }

  [HttpGet("interview-stats")]
  public async Task<IActionResult> GetInterviewStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
  {
    try
    {
      var stats = await _reportsRepository.GetInterviewStatsAsync(startDate, endDate);
      return Ok(stats);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving interview statistics.", error = ex.Message });
    }
  }

  [HttpGet("source-analysis")]
  public async Task<IActionResult> GetSourceAnalysis()
  {
    try
    {
      var analysis = await _reportsRepository.GetSourceAnalysisAsync();
      return Ok(analysis);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving source analysis.", error = ex.Message });
    }
  }

  [HttpGet("monthly-trends")]
  public async Task<IActionResult> GetMonthlyTrends([FromQuery] int months = 12)
  {
    try
    {
      var trends = await _reportsRepository.GetMonthlyTrendsAsync(months);
      return Ok(trends);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving monthly trends.", error = ex.Message });
    }
  }

  [HttpGet("skill-demand")]
  public async Task<IActionResult> GetSkillDemand()
  {
    try
    {
      var demand = await _reportsRepository.GetSkillDemandAsync();
      return Ok(demand);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving skill demand.", error = ex.Message });
    }
  }

  [HttpGet("application-funnel")]
  public async Task<IActionResult> GetApplicationFunnel([FromQuery] Guid? jobId)
  {
    try
    {
      var funnel = await _reportsRepository.GetApplicationFunnelAsync(jobId);
      return Ok(funnel);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving application funnel.", error = ex.Message });
    }
  }

  [HttpGet("experience-wise-candidates")]
  public async Task<IActionResult> GetExperienceWiseCandidates()
  {
    try
    {
      var candidates = await _reportsRepository.GetExperienceWiseCandidatesAsync();
      return Ok(candidates);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving experience-wise candidates.", error = ex.Message });
    }
  }

  [HttpGet("college-wise")]
  public async Task<IActionResult> GetCollegeWiseReport()
  {
    try
    {
      var report = await _reportsRepository.GetCollegeWiseReportAsync();
      return Ok(report);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving college-wise report.", error = ex.Message });
    }
  }
}
