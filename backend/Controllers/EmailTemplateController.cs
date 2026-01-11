using backend.DTOs.EmailTemplate;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class EmailTemplateController : ControllerBase
{
  private readonly IEmailTemplateRepository _templateRepository;
  private readonly IEmailService _emailService;
  private readonly ILogger<EmailTemplateController> _logger;

  public EmailTemplateController(
    IEmailTemplateRepository templateRepository,
    IEmailService emailService,
    ILogger<EmailTemplateController> logger)
  {
    _templateRepository = templateRepository;
    _emailService = emailService;
    _logger = logger;
  }

  [HttpGet]
  [Authorize(Policy = "ViewSettings")]
  public async Task<ActionResult<List<EmailTemplateDto>>> GetAllTemplates()
  {
    try
    {
      var templates = await _templateRepository.GetAllAsync();
      return Ok(templates);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving email templates");
      return StatusCode(500, "An error occurred while retrieving templates");
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewSettings")]
  public async Task<ActionResult<EmailTemplateDto>> GetTemplateById(Guid id)
  {
    try
    {
      var template = await _templateRepository.GetByIdAsync(id);
      if (template == null)
        return NotFound($"The email template with ID {id} could not be found");

      return Ok(template);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving template {TemplateId}", id);
      return StatusCode(500, "An error occurred while retrieving the template");
    }
  }

  [HttpGet("category/{category}")]
  [Authorize(Policy = "ViewSettings")]
  public async Task<ActionResult<List<EmailTemplateDto>>> GetTemplatesByCategory(string category)
  {
    try
    {
      var templates = await _templateRepository.GetByCategoryAsync(category);
      return Ok(templates);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving templates for category {Category}", category);
      return StatusCode(500, "An error occurred while retrieving templates");
    }
  }

  [HttpGet("active")]
  [Authorize(Policy = "ViewSettings")]
  public async Task<ActionResult<List<EmailTemplateDto>>> GetActiveTemplates()
  {
    try
    {
      var templates = await _templateRepository.GetActiveTemplatesAsync();
      return Ok(templates);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving active templates");
      return StatusCode(500, "An error occurred while retrieving templates");
    }
  }

  [HttpPost]
  [Authorize(Policy = "ManageSettings")]
  public async Task<ActionResult<EmailTemplateDto>> CreateTemplate([FromBody] CreateEmailTemplateDto dto)
  {
    try
    {
      if (await _templateRepository.ExistsByNameAsync(dto.Name))
        return BadRequest($"An email template with the name '{dto.Name}' already exists. Please use a different name");

      var template = await _templateRepository.CreateAsync(dto);
      return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating email template");
      return StatusCode(500, "An error occurred while creating the template");
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageSettings")]
  public async Task<ActionResult<EmailTemplateDto>> UpdateTemplate(Guid id, [FromBody] UpdateEmailTemplateDto dto)
  {
    try
    {
      var template = await _templateRepository.UpdateAsync(id, dto);
      if (template == null)
        return NotFound($"The email template with ID {id} could not be found");

      return Ok(template);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating template {TemplateId}", id);
      return StatusCode(500, "An error occurred while updating the template");
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageSettings")]
  public async Task<ActionResult> DeleteTemplate(Guid id)
  {
    try
    {
      var success = await _templateRepository.DeleteAsync(id);
      if (!success)
        return NotFound($"The email template with ID {id} could not be found");

      return NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting template {TemplateId}", id);
      return StatusCode(500, "An error occurred while deleting the template");
    }
  }

  [HttpPost("preview")]
  [Authorize(Policy = "ViewSettings")]
  public async Task<ActionResult<TemplatePreviewResultDto>> PreviewTemplate([FromBody] PreviewTemplateDto dto)
  {
    try
    {
      var template = await _templateRepository.GetByIdAsync(dto.TemplateId);
      if (template == null)
        return NotFound($"The email template with ID {dto.TemplateId} could not be found");

      var subject = await _emailService.ApplyVariablesToTemplate(template.Subject, dto.Variables);
      var body = await _emailService.ApplyVariablesToTemplate(template.Body, dto.Variables);

      return Ok(new TemplatePreviewResultDto(subject, body));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error previewing template {TemplateId}", dto.TemplateId);
      return StatusCode(500, "An error occurred while previewing the template");
    }
  }

  [HttpPost("send")]
  [Authorize(Policy = "SendEmails")]
  public async Task<ActionResult> SendEmailWithTemplate([FromBody] ApplyTemplateDto dto)
  {
    try
    {
      var success = await _emailService.SendEmailWithTemplateAsync(
        dto.TemplateId,
        dto.ToEmail,
        dto.ToName,
        dto.Variables
      );

      if (!success)
        return BadRequest("Unable to send email. Please check the email address and try again, or contact support");

      return Ok(new { message = "Email sent successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error sending email with template {TemplateId} to {Email}", dto.TemplateId, dto.ToEmail);
      return StatusCode(500, "An error occurred while sending the email");
    }
  }
}
