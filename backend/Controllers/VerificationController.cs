using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Repositories.IRepositories;
using backend.DTOs.Verification;
using backend.Services.IServices;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("verifications")]
[Authorize]
public class VerificationController(
  IVerificationRepository verificationRepository,
  IEmailService emailService,
  ICandidateRepository candidateRepository,
  IEmployeeRepository employeeRepository) : ControllerBase
{
  private readonly IVerificationRepository _verificationRepository = verificationRepository;
  private readonly IEmailService _emailService = emailService;
  private readonly ICandidateRepository _candidateRepository = candidateRepository;
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;

  [HttpGet]
  [Authorize(Policy = "VerifyDocuments")]
  public async Task<IActionResult> GetAllVerifications()
  {
    try
    {
      var verifications = await _verificationRepository.GetAllVerificationsAsync();
      return Ok(verifications);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving verifications.", error = ex.Message });
    }
  }

  [HttpGet("candidate/{candidateId}")]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<IActionResult> GetVerificationsByCandidate(Guid candidateId)
  {
    try
    {
      var verifications = await _verificationRepository.GetVerificationsByCandidateAsync(candidateId);
      return Ok(verifications);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving candidate verifications.", error = ex.Message });
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "VerifyDocuments")]
  public async Task<IActionResult> GetVerification(Guid id)
  {
    try
    {
      var verification = await _verificationRepository.GetVerificationByIdAsync(id);
      if (verification == null)
        return NotFound(new { message = "The verification record could not be found" });

      return Ok(verification);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the verification.", error = ex.Message });
    }
  }

  [HttpPost]
  [Authorize(Policy = "VerifyDocuments")]
  public async Task<IActionResult> CreateVerification([FromBody] CreateVerificationDto dto)
  {
    try
    {
      // Resolve User ID to Employee ID
      var employee = await _employeeRepository.GetEmployeeByUserIdAsync(dto.VerifiedBy);
      if (employee == null)
      {
        return BadRequest(new { message = "Only employees can verify documents. The current user is not registered as an employee." });
      }

      // Create new DTO with Employee ID
      var verificationDtoWithEmployeeId = new CreateVerificationDto(
        dto.CandidateId,
        dto.DocumentId,
        dto.StatusId,
        dto.Comments,
        employee.Id
      );

      var verification = await _verificationRepository.CreateVerificationAsync(verificationDtoWithEmployeeId);
      return CreatedAtAction(nameof(GetVerification), new { id = verification.Id }, verification);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the verification.", error = ex.Message });
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "VerifyDocuments")]
  public async Task<IActionResult> UpdateVerification(Guid id, [FromBody] UpdateVerificationDto dto)
  {
    try
    {
      var verification = await _verificationRepository.UpdateVerificationAsync(id, dto);
      if (verification == null)
        return NotFound(new { message = "The verification record you're trying to update could not be found" });

      // Send email notification for verification status change
      _ = SendVerificationStatusEmailAsync(verification.CandidateId, verification.Status ?? "Updated", verification.Comments);

      return Ok(verification);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the verification.", error = ex.Message });
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "VerifyDocuments")]
  public async Task<IActionResult> DeleteVerification(Guid id)
  {
    try
    {
      var result = await _verificationRepository.DeleteVerificationAsync(id);
      if (!result)
        return NotFound(new { message = "The verification record you're trying to delete could not be found" });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the verification.", error = ex.Message });
    }
  }

  private async Task SendVerificationStatusEmailAsync(Guid candidateId, string status, string? comments)
  {
    try
    {
      var candidate = await _candidateRepository.GetByIdAsync(candidateId);
      if (candidate?.User?.Email == null) return;

      await _emailService.SendApplicationStatusUpdateAsync(
        candidate.User.Email,
        candidate.FullName ?? "Candidate",
        "Document Verification",
        $"Your document verification status: {status}",
        comments
      );
    }
    catch (Exception ex)
    {
      // Log error but don't fail the request
      Console.WriteLine($"Failed to send verification status email: {ex.Message}");
    }
  }
}
