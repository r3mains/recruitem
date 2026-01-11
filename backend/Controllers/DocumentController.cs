using backend.DTOs.Document;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using backend.Validators.DocumentValidators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("documents")]
[Authorize]
public class DocumentController(
  IDocumentRepository documentRepository,
  IFileStorageService fileStorageService,
  UploadDocumentValidator uploadValidator,
  UpdateDocumentValidator updateValidator) : ControllerBase
{
  private readonly IDocumentRepository _documentRepository = documentRepository;
  private readonly IFileStorageService _fileStorageService = fileStorageService;
  private readonly UploadDocumentValidator _uploadValidator = uploadValidator;
  private readonly UpdateDocumentValidator _updateValidator = updateValidator;

  [HttpGet]
  [Authorize(Policy = "ViewDocuments")]
  public async Task<IActionResult> GetAllDocuments()
  {
    try
    {
      var documents = await _documentRepository.GetAllDocumentsAsync();
      return Ok(documents);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving documents.", error = ex.Message });
    }
  }

  [HttpGet("candidate/{candidateId}")]
  [Authorize(Policy = "ViewDocuments")]
  public async Task<IActionResult> GetDocumentsByCandidate(Guid candidateId)
  {
    try
    {
      var documents = await _documentRepository.GetDocumentsByCandidateAsync(candidateId);
      return Ok(documents);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving documents.", error = ex.Message });
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewDocuments")]
  public async Task<IActionResult> GetDocumentById(Guid id)
  {
    try
    {
      var document = await _documentRepository.GetDocumentByIdAsync(id);
      if (document == null)
        return NotFound(new { message = "The requested document could not be found" });

      return Ok(document);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the document.", error = ex.Message });
    }
  }

  [HttpPost("upload")]
  [Authorize(Policy = "UploadDocuments")]
  [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
  public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentDto dto)
  {
    try
    {
      var validationResult = await _uploadValidator.ValidateAsync(dto);
      if (!validationResult.IsValid)
      {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return BadRequest(new { message = "Unable to upload document. " + string.Join(" ", errors), errors });
      }

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var uploadedBy = userId != null ? Guid.Parse(userId) : (Guid?)null;

      // Save file
      var filePath = await _fileStorageService.SaveFileAsync(dto.File, "documents");

      // Create document record
      var document = await _documentRepository.CreateDocumentAsync(
        dto.CandidateId,
        dto.DocumentTypeId,
        filePath,
        dto.File.FileName,
        dto.File.ContentType,
        dto.File.Length,
        uploadedBy
      );

      return CreatedAtAction(nameof(GetDocumentById), new { id = document.Id }, document);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while uploading the document.", error = ex.Message });
    }
  }

  [HttpGet("{id}/download")]
  [Authorize(Policy = "ViewDocuments")]
  public async Task<IActionResult> DownloadDocument(Guid id)
  {
    try
    {
      var document = await _documentRepository.GetDocumentByIdAsync(id);
      if (document == null)
        return NotFound(new { message = "The requested document could not be found" });

      if (!_fileStorageService.FileExists(document.Url))
        return NotFound(new { message = "The document file could not be found on the server. Please contact support" });

      var fileStream = await _fileStorageService.GetFileStreamAsync(document.Url);
      var contentType = _fileStorageService.GetContentType(document.Url);
      var fileName = document.OriginalFileName ?? "document";

      return File(fileStream, contentType, fileName);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while downloading the document.", error = ex.Message });
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageDocuments")]
  public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentDto dto)
  {
    try
    {
      var validationResult = await _updateValidator.ValidateAsync(dto);
      if (!validationResult.IsValid)
      {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return BadRequest(new { message = "Unable to update document. " + string.Join(" ", errors), errors });
      }

      var document = await _documentRepository.UpdateDocumentAsync(id, dto);
      if (document == null)
        return NotFound(new { message = "The document you're trying to update could not be found" });

      return Ok(document);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the document.", error = ex.Message });
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageDocuments")]
  public async Task<IActionResult> DeleteDocument(Guid id)
  {
    try
    {
      var document = await _documentRepository.GetDocumentByIdAsync(id);
      if (document == null)
        return NotFound(new { message = "The document you're trying to delete could not be found" });

      // Delete file from storage
      await _fileStorageService.DeleteFileAsync(document.Url);

      // Delete database record
      await _documentRepository.DeleteDocumentAsync(id);

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the document.", error = ex.Message });
    }
  }

  // Document Types
  [HttpGet("types")]
  public async Task<IActionResult> GetDocumentTypes()
  {
    try
    {
      var types = await _documentRepository.GetAllDocumentTypesAsync();
      return Ok(types);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving document types.", error = ex.Message });
    }
  }

  [HttpGet("types/{id}")]
  public async Task<IActionResult> GetDocumentTypeById(Guid id)
  {
    try
    {
      var type = await _documentRepository.GetDocumentTypeByIdAsync(id);
      if (type == null)
        return NotFound(new { message = "The requested document type could not be found" });

      return Ok(type);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the document type.", error = ex.Message });
    }
  }

  [HttpPost("types")]
  [Authorize(Policy = "ManageDocuments")]
  public async Task<IActionResult> CreateDocumentType([FromBody] CreateDocumentTypeDto dto)
  {
    try
    {
      var validator = new CreateDocumentTypeValidator();
      var validationResult = await validator.ValidateAsync(dto);
      if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);

      var type = await _documentRepository.CreateDocumentTypeAsync(dto);
      return CreatedAtAction(nameof(GetDocumentTypeById), new { id = type.Id }, type);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the document type.", error = ex.Message });
    }
  }

  [HttpPut("types/{id}")]
  [Authorize(Policy = "ManageDocuments")]
  public async Task<IActionResult> UpdateDocumentType(Guid id, [FromBody] UpdateDocumentTypeDto dto)
  {
    try
    {
      var validator = new UpdateDocumentTypeValidator();
      var validationResult = await validator.ValidateAsync(dto);
      if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);

      var type = await _documentRepository.UpdateDocumentTypeAsync(id, dto);
      if (type == null)
        return NotFound(new { message = "The document type you're trying to update could not be found" });

      return Ok(type);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the document type.", error = ex.Message });
    }
  }

  [HttpDelete("types/{id}")]
  [Authorize(Policy = "ManageDocuments")]
  public async Task<IActionResult> DeleteDocumentType(Guid id)
  {
    try
    {
      var result = await _documentRepository.DeleteDocumentTypeAsync(id);
      if (!result)
        return NotFound(new { message = "The document type you're trying to delete could not be found" });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the document type.", error = ex.Message });
    }
  }
}
