using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Candidate;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Services.IServices;
using backend.Validators.CandidateValidators;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "ViewCandidates")]
public class CandidateController(
  ICandidateRepository candidateRepository,
  IMapper mapper,
  UserManager<User> userManager,
  CreateCandidateValidator createCandidateValidator,
  UpdateCandidateValidator updateCandidateValidator,
  ICandidateBulkImportService candidateBulkImportService) : ControllerBase
{
  private readonly ICandidateRepository _candidateRepository = candidateRepository;
  private readonly IMapper _mapper = mapper;
  private readonly UserManager<User> _userManager = userManager;
  private readonly CreateCandidateValidator _createCandidateValidator = createCandidateValidator;
  private readonly UpdateCandidateValidator _updateCandidateValidator = updateCandidateValidator;
  private readonly ICandidateBulkImportService _candidateBulkImportService = candidateBulkImportService;

  // GET /api/v1/candidate
  [HttpGet]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<IActionResult> GetCandidates(
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Invalid pagination parameters. Page must be 1 or greater, and page size must be between 1 and 100");
    }

    var candidates = await _candidateRepository.GetAllAsync(search, page, pageSize);
    var candidateListDtos = _mapper.Map<IEnumerable<CandidateListDto>>(candidates);

    var totalCount = await _candidateRepository.GetCountAsync(search);
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    var result = new
    {
      Candidates = candidateListDtos,
      Pagination = new
      {
        CurrentPage = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = totalPages
      }
    };

    return Ok(result);
  }

  // GET /api/v1/candidate/{id}
  [HttpGet("{id}")]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<IActionResult> GetCandidate(Guid id)
  {
    var candidate = await _candidateRepository.GetByIdAsync(id);
    if (candidate == null)
    {
      return NotFound($"The candidate profile with ID {id} could not be found");
    }

    var candidateResponseDto = _mapper.Map<CandidateResponseDto>(candidate);
    return Ok(candidateResponseDto);
  }

  [HttpPost]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<IActionResult> CreateCandidate(CreateCandidateDto createCandidateDto)
  {
    var validationResult = await _createCandidateValidator.ValidateAsync(createCandidateDto);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
      return BadRequest(new { message = "Unable to create candidate profile. " + string.Join(" ", errors), errors });
    }

    if (await _candidateRepository.ExistsByEmailAsync(createCandidateDto.Email))
    {
      return BadRequest($"A candidate with the email address '{createCandidateDto.Email}' already exists. Please use a different email");
    }

    var user = new User
    {
      UserName = createCandidateDto.Email,
      Email = createCandidateDto.Email,
      EmailConfirmed = true,
      CreatedAt = DateTimeOffset.UtcNow,
      UpdatedAt = DateTimeOffset.UtcNow
    };

    var userResult = await _userManager.CreateAsync(user, createCandidateDto.Password);
    if (!userResult.Succeeded)
    {
      var errors = userResult.Errors.Select(e => e.Description).ToList();
      return BadRequest(new { message = "Unable to create user account. " + string.Join(" ", errors), errors });
    }

    var candidate = _mapper.Map<Candidate>(createCandidateDto);
    candidate.UserId = user.Id;
    candidate.CreatedAt = DateTime.UtcNow;
    candidate.UpdatedAt = DateTime.UtcNow;

    if (createCandidateDto.Address != null)
    {
      var address = _mapper.Map<Address>(createCandidateDto.Address);
      candidate.Address = address;
    }

    var createdCandidate = await _candidateRepository.CreateAsync(candidate);

    if (createCandidateDto.Skills != null && createCandidateDto.Skills.Any())
    {
      foreach (var skillDto in createCandidateDto.Skills)
      {
        var candidateSkill = _mapper.Map<CandidateSkill>(skillDto);
        candidateSkill.CandidateId = createdCandidate.Id;
        await _candidateRepository.AddSkillAsync(candidateSkill);
      }
    }

    if (createCandidateDto.Qualifications != null && createCandidateDto.Qualifications.Any())
    {
      foreach (var qualificationDto in createCandidateDto.Qualifications)
      {
        var candidateQualification = _mapper.Map<CandidateQualification>(qualificationDto);
        candidateQualification.CandidateId = createdCandidate.Id;
        await _candidateRepository.AddQualificationAsync(candidateQualification);
      }
    }

    var fullCandidate = await _candidateRepository.GetByIdAsync(createdCandidate.Id);
    var candidateResponseDto = _mapper.Map<CandidateResponseDto>(fullCandidate);

    return CreatedAtAction(nameof(GetCandidate), new { id = createdCandidate.Id }, candidateResponseDto);
  }

  // PUT /api/v1/candidate/{id}
  [HttpPut("{id}")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<IActionResult> UpdateCandidate(Guid id, UpdateCandidateDto updateCandidateDto)
  {
    var validationResult = await _updateCandidateValidator.ValidateAsync(updateCandidateDto);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
      return BadRequest(new { message = "Unable to update candidate profile. " + string.Join(" ", errors), errors });
    }

    var existingCandidate = await _candidateRepository.GetByIdAsync(id);
    if (existingCandidate == null)
    {
      return NotFound($"The candidate profile with ID {id} could not be found");
    }

    _mapper.Map(updateCandidateDto, existingCandidate);
    existingCandidate.UpdatedAt = DateTime.UtcNow;

    if (updateCandidateDto.Address != null)
    {
      if (existingCandidate.Address != null)
      {
        _mapper.Map(updateCandidateDto.Address, existingCandidate.Address);
      }
      else
      {
        existingCandidate.Address = _mapper.Map<Address>(updateCandidateDto.Address);
      }
    }

    var updatedCandidate = await _candidateRepository.UpdateAsync(existingCandidate);

    if (updateCandidateDto.Skills != null)
    {
      await _candidateRepository.RemoveAllSkillsAsync(id);
      foreach (var skillDto in updateCandidateDto.Skills)
      {
        var candidateSkill = _mapper.Map<CandidateSkill>(skillDto);
        candidateSkill.CandidateId = id;
        await _candidateRepository.AddSkillAsync(candidateSkill);
      }
    }

    if (updateCandidateDto.Qualifications != null)
    {
      await _candidateRepository.RemoveAllQualificationsAsync(id);
      foreach (var qualificationDto in updateCandidateDto.Qualifications)
      {
        var candidateQualification = _mapper.Map<CandidateQualification>(qualificationDto);
        candidateQualification.CandidateId = id;
        await _candidateRepository.AddQualificationAsync(candidateQualification);
      }
    }

    var fullCandidate = await _candidateRepository.GetByIdAsync(updatedCandidate.Id);
    var candidateResponseDto = _mapper.Map<CandidateResponseDto>(fullCandidate);

    return Ok(candidateResponseDto);
  }

  // PUT /api/v1/candidate/profile
  [HttpPut("profile")]
  [Authorize(Roles = "Candidate")]
  public async Task<IActionResult> UpdateMyProfile(UpdateCandidateDto updateCandidateDto)
  {
    var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value!);
    var candidate = await _candidateRepository.GetByUserIdAsync(userId);

    if (candidate == null)
    {
      return BadRequest("Your candidate profile could not be found. Please contact support");
    }

    return await UpdateCandidate(candidate.Id, updateCandidateDto);
  }

  // GET /api/v1/candidate/profile
  [HttpGet("profile")]
  [Authorize(Roles = "Candidate")]
  public async Task<IActionResult> GetMyProfile()
  {
    var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value!);
    var candidate = await _candidateRepository.GetByUserIdAsync(userId);

    if (candidate == null)
    {
      return NotFound("Your candidate profile could not be found. Please contact support");
    }

    var candidateResponseDto = _mapper.Map<CandidateResponseDto>(candidate);
    return Ok(candidateResponseDto);
  }

  // DELETE /api/v1/candidate/{id}
  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult> DeleteCandidate(Guid id)
  {
    var candidate = await _candidateRepository.GetByIdAsync(id);
    if (candidate == null)
    {
      return NotFound($"The candidate profile with ID {id} could not be found");
    }

    await _candidateRepository.DeleteAsync(id);
    return NoContent();
  }

  // GET /api/v1/candidate/bulk-upload-template
  [HttpGet("bulk-upload-template")]
  [Authorize(Policy = "ManageCandidates")]
  public IActionResult DownloadBulkUploadTemplate()
  {
    try
    {
      OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
      
      using var package = new OfficeOpenXml.ExcelPackage();
      var worksheet = package.Workbook.Worksheets.Add("Candidates");

      // Add headers
      worksheet.Cells[1, 1].Value = "Full Name *";
      worksheet.Cells[1, 2].Value = "Email *";
      worksheet.Cells[1, 3].Value = "Contact Number";
      worksheet.Cells[1, 4].Value = "Skills (comma-separated)";
      worksheet.Cells[1, 5].Value = "Qualifications (comma-separated)";
      worksheet.Cells[1, 6].Value = "City";
      worksheet.Cells[1, 7].Value = "State";
      worksheet.Cells[1, 8].Value = "Country";

      // Style headers
      using (var range = worksheet.Cells[1, 1, 1, 8])
      {
        range.Style.Font.Bold = true;
        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
      }

      // Add sample data
      worksheet.Cells[2, 1].Value = "John Doe";
      worksheet.Cells[2, 2].Value = "john.doe@example.com";
      worksheet.Cells[2, 3].Value = "+1234567890";
      worksheet.Cells[2, 4].Value = "C#, .NET, SQL";
      worksheet.Cells[2, 5].Value = "B.Tech, M.Tech";
      worksheet.Cells[2, 6].Value = "New York";
      worksheet.Cells[2, 7].Value = "NY";
      worksheet.Cells[2, 8].Value = "USA";

      // Auto-fit columns
      worksheet.Cells.AutoFitColumns();

      var stream = new MemoryStream(package.GetAsByteArray());
      return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CandidateBulkUploadTemplate.xlsx");
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error generating template: {ex.Message}");
    }
  }

  // POST /api/v1/candidate/bulk-upload
  [HttpPost("bulk-upload")]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<IActionResult> BulkUploadCandidates(IFormFile file)
  {
    if (file == null || file.Length == 0)
    {
      return BadRequest("Please upload a valid Excel file");
    }

    var allowedExtensions = new[] { ".xlsx", ".xls" };
    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (!allowedExtensions.Contains(fileExtension))
    {
      return BadRequest("Only Excel files (.xlsx, .xls) are allowed");
    }

    if (file.Length > 10 * 1024 * 1024) // 10MB
    {
      return BadRequest("File size should not exceed 10MB");
    }

    try
    {
      using var stream = file.OpenReadStream();
      var result = await _candidateBulkImportService.ImportCandidatesFromExcelAsync(stream, file.FileName);

      return Ok(new
      {
        Message = $"Import completed. Success: {result.SuccessCount}, Failed: {result.FailureCount}",
        TotalRecords = result.TotalRecords,
        SuccessCount = result.SuccessCount,
        FailureCount = result.FailureCount,
        Errors = result.Errors,
        Warnings = result.Warnings,
        CreatedCandidateIds = result.CreatedCandidateIds
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error processing file: {ex.Message}");
    }
  }
}
