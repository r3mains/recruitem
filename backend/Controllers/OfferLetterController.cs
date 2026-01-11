using backend.DTOs.OfferLetter;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class OfferLetterController : ControllerBase
{
  private readonly IOfferLetterRepository _offerRepository;
  private readonly IPdfGenerationService _pdfService;
  private readonly IEmailService _emailService;
  private readonly IConfiguration _configuration;
  private readonly ILogger<OfferLetterController> _logger;

  public OfferLetterController(
    IOfferLetterRepository offerRepository,
    IPdfGenerationService pdfService,
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<OfferLetterController> logger)
  {
    _offerRepository = offerRepository;
    _pdfService = pdfService;
    _emailService = emailService;
    _configuration = configuration;
    _logger = logger;
  }

  [HttpGet]
  [Authorize(Policy = "ViewOfferLetters")]
  public async Task<ActionResult<List<OfferLetterDto>>> GetAllOffers()
  {
    try
    {
      var offers = await _offerRepository.GetAllAsync();
      return Ok(offers);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving offer letters");
      return StatusCode(500, "An error occurred while retrieving offer letters");
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewOfferLetters")]
  public async Task<ActionResult<OfferLetterDto>> GetOfferById(Guid id)
  {
    try
    {
      var offer = await _offerRepository.GetByIdAsync(id);
      if (offer == null)
        return NotFound($"The offer letter with ID {id} could not be found");

      return Ok(offer);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving offer letter {OfferId}", id);
      return StatusCode(500, "An error occurred while retrieving the offer letter");
    }
  }

  [HttpGet("application/{applicationId}")]
  [Authorize(Policy = "ViewOfferLetters")]
  public async Task<ActionResult<List<OfferLetterDto>>> GetOffersByApplication(Guid applicationId)
  {
    try
    {
      var offers = await _offerRepository.GetByJobApplicationIdAsync(applicationId);
      return Ok(offers);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving offers for application {ApplicationId}", applicationId);
      return StatusCode(500, "An error occurred while retrieving offer letters");
    }
  }

  [HttpGet("status/{status}")]
  [Authorize(Policy = "ViewOfferLetters")]
  public async Task<ActionResult<List<OfferLetterDto>>> GetOffersByStatus(string status)
  {
    try
    {
      var offers = await _offerRepository.GetByStatusAsync(status);
      return Ok(offers);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving offers with status {Status}", status);
      return StatusCode(500, "An error occurred while retrieving offer letters");
    }
  }

  [HttpPost]
  [Authorize(Policy = "ManageOfferLetters")]
  public async Task<ActionResult<OfferLetterDto>> CreateOffer([FromBody] CreateOfferLetterDto dto)
  {
    try
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId))
        return Unauthorized("Unable to verify your identity. Please log in again");

      var offer = await _offerRepository.CreateAsync(dto, Guid.Parse(userId));
      return CreatedAtAction(nameof(GetOfferById), new { id = offer.Id }, offer);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating offer letter");
      return StatusCode(500, "An error occurred while creating the offer letter");
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageOfferLetters")]
  public async Task<ActionResult<OfferLetterDto>> UpdateOffer(Guid id, [FromBody] UpdateOfferLetterDto dto)
  {
    try
    {
      var offer = await _offerRepository.UpdateAsync(id, dto);
      if (offer == null)
        return NotFound($"The offer letter with ID {id} could not be found");

      return Ok(offer);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating offer letter {OfferId}", id);
      return StatusCode(500, "An error occurred while updating the offer letter");
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageOfferLetters")]
  public async Task<ActionResult> DeleteOffer(Guid id)
  {
    try
    {
      var success = await _offerRepository.DeleteAsync(id);
      if (!success)
        return NotFound($"The offer letter with ID {id} could not be found");

      return NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting offer letter {OfferId}", id);
      return StatusCode(500, "An error occurred while deleting the offer letter");
    }
  }

  [HttpPost("generate")]
  [Authorize(Policy = "ManageOfferLetters")]
  public async Task<ActionResult> GenerateOfferLetterPdf([FromBody] GenerateOfferLetterDto dto)
  {
    try
    {
      var offer = await _offerRepository.GetByIdAsync(dto.OfferLetterId);
      if (offer == null)
        return NotFound($"The offer letter with ID {dto.OfferLetterId} could not be found");

      var companyName = dto.CompanyName ?? _configuration["Company:Name"] ?? "Recruitment Company";
      var companyAddress = dto.CompanyAddress ?? _configuration["Company:Address"];
      var signatoryName = dto.SignatoryName ?? _configuration["Company:SignatoryName"];
      var signatoryDesignation = dto.SignatoryDesignation ?? _configuration["Company:SignatoryDesignation"];

      var pdfPath = await _pdfService.GenerateOfferLetterPdfAsync(
        offer.CandidateName,
        offer.JobTitle,
        companyName,
        companyAddress,
        offer.Salary,
        offer.JoiningDate ?? DateTime.UtcNow.AddDays(30),
        offer.OfferDate,
        offer.Benefits,
        offer.AdditionalTerms,
        signatoryName,
        signatoryDesignation
      );

      await _offerRepository.UpdatePdfPathAsync(dto.OfferLetterId, pdfPath);

      return Ok(new { message = "Offer letter PDF generated successfully", pdfPath });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error generating offer letter PDF {OfferId}", dto.OfferLetterId);
      return StatusCode(500, "An error occurred while generating the PDF");
    }
  }

  [HttpPost("send")]
  [Authorize(Policy = "ManageOfferLetters")]
  public async Task<ActionResult> SendOfferLetter([FromBody] GenerateOfferLetterDto dto)
  {
    try
    {
      var offer = await _offerRepository.GetByIdAsync(dto.OfferLetterId);
      if (offer == null)
        return NotFound($"The offer letter with ID {dto.OfferLetterId} could not be found");

      // Generate PDF if not already generated
      if (string.IsNullOrEmpty(offer.GeneratedPdfPath))
      {
        var generateResult = await GenerateOfferLetterPdf(dto);
        if (generateResult is not OkObjectResult)
          return generateResult;
      }

      // Send email
      var success = await _emailService.SendOfferLetterAsync(
        offer.CandidateEmail,
        offer.CandidateName,
        offer.JobTitle,
        $"Please find attached your offer letter. The position offers an annual compensation of ${offer.Salary:N2}."
      );

      if (!success)
        return BadRequest("Unable to send offer letter email. Please check the email address and try again, or contact support");

      return Ok(new { message = "Offer letter sent successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error sending offer letter {OfferId}", dto.OfferLetterId);
      return StatusCode(500, "An error occurred while sending the offer letter");
    }
  }

  [HttpGet("{id}/download")]
  [Authorize(Policy = "ViewOfferLetters")]
  public async Task<ActionResult> DownloadOfferLetter(Guid id)
  {
    try
    {
      var offer = await _offerRepository.GetByIdAsync(id);
      if (offer == null)
        return NotFound($"The offer letter with ID {id} could not be found");

      if (string.IsNullOrEmpty(offer.GeneratedPdfPath))
        return BadRequest("The offer letter PDF has not been generated yet. Please generate it first");

      var pdfBytes = await _pdfService.GetPdfBytesAsync(offer.GeneratedPdfPath);
      
      return File(pdfBytes, "application/pdf", $"OfferLetter_{offer.CandidateName.Replace(" ", "_")}.pdf");
    }
    catch (FileNotFoundException)
    {
      return NotFound("The offer letter PDF file could not be found. Please regenerate the offer letter");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error downloading offer letter {OfferId}", id);
      return StatusCode(500, "An error occurred while downloading the offer letter");
    }
  }

  [HttpPost("accept")]
  [Authorize]
  public async Task<ActionResult> AcceptOffer([FromBody] AcceptOfferDto dto)
  {
    try
    {
      var success = await _offerRepository.AcceptOfferAsync(dto.OfferLetterId);
      if (!success)
        return NotFound($"The offer letter with ID {dto.OfferLetterId} could not be found");

      return Ok(new { message = "Offer accepted successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error accepting offer {OfferId}", dto.OfferLetterId);
      return StatusCode(500, "An error occurred while accepting the offer");
    }
  }

  [HttpPost("reject")]
  [Authorize]
  public async Task<ActionResult> RejectOffer([FromBody] RejectOfferDto dto)
  {
    try
    {
      var success = await _offerRepository.RejectOfferAsync(dto.OfferLetterId, dto.Reason);
      if (!success)
        return NotFound($"The offer letter with ID {dto.OfferLetterId} could not be found");

      return Ok(new { message = "Offer rejected successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error rejecting offer {OfferId}", dto.OfferLetterId);
      return StatusCode(500, "An error occurred while rejecting the offer");
    }
  }
}
