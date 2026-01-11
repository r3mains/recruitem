using backend.DTOs.Resume;
using backend.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("resume")]
[Authorize]
public class ResumeController : ControllerBase
{
  private readonly IResumeParserService _resumeParserService;
  private readonly ILogger<ResumeController> _logger;

  public ResumeController(
    IResumeParserService resumeParserService,
    ILogger<ResumeController> logger)
  {
    _resumeParserService = resumeParserService;
    _logger = logger;
  }

  [HttpPost("parse")]
  [Authorize(Policy = "ManageCandidates")]
  [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
  public async Task<IActionResult> ParseResume([FromForm] ParseResumeDto dto)
  {
    try
    {
      if (dto.ResumeFile == null || dto.ResumeFile.Length == 0)
      {
        return BadRequest(new { message = "Please upload a resume file. The file cannot be empty" });
      }

      var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
      var extension = Path.GetExtension(dto.ResumeFile.FileName).ToLowerInvariant();
      
      if (!allowedExtensions.Contains(extension))
      {
        return BadRequest(new { message = "Unsupported file format. Please upload a PDF, DOC, DOCX, or TXT file" });
      }

      var parsedResume = await _resumeParserService.ParseResumeAsync(dto.ResumeFile);

      return Ok(parsedResume);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error parsing resume");
      return StatusCode(500, new { message = "An error occurred while parsing the resume.", error = ex.Message });
    }
  }
}
