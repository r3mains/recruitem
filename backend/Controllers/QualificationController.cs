using AutoMapper;
using backend.DTOs.Qualification;
using backend.Repositories.IRepositories;
using backend.Validators.QualificationValidators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class QualificationController(
  IQualificationRepository qualificationRepository,
  IMapper mapper,
  CreateQualificationValidator createQualificationValidator,
  UpdateQualificationValidator updateQualificationValidator) : ControllerBase
{
  private readonly IQualificationRepository _qualificationRepository = qualificationRepository;
  private readonly IMapper _mapper = mapper;
  private readonly CreateQualificationValidator _createQualificationValidator = createQualificationValidator;
  private readonly UpdateQualificationValidator _updateQualificationValidator = updateQualificationValidator;

  [HttpGet]
  public async Task<ActionResult<IEnumerable<QualificationListDto>>> GetQualifications(
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Invalid pagination parameters. Page must be 1 or greater, and page size must be between 1 and 100");
    }

    var qualifications = await _qualificationRepository.GetAllAsync(search, page, pageSize);
    var qualificationListDtos = _mapper.Map<IEnumerable<QualificationListDto>>(qualifications);

    return Ok(qualificationListDtos);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<QualificationResponseDto>> GetQualification(Guid id)
  {
    var qualification = await _qualificationRepository.GetByIdAsync(id);
    if (qualification == null)
    {
      return NotFound($"The qualification with ID {id} could not be found");
    }

    var qualificationResponseDto = _mapper.Map<QualificationResponseDto>(qualification);
    return Ok(qualificationResponseDto);
  }

  [HttpPost]
  [Authorize(Policy = "AdminPolicy")]
  public async Task<ActionResult<QualificationResponseDto>> CreateQualification(CreateQualificationDto createQualificationDto)
  {
    var validationResult = await _createQualificationValidator.ValidateAsync(createQualificationDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    if (await _qualificationRepository.ExistsByNameAsync(createQualificationDto.QualificationName))
    {
      return BadRequest($"The qualification '{createQualificationDto.QualificationName}' already exists. Please use a different name");
    }

    var qualification = _mapper.Map<backend.Models.Qualification>(createQualificationDto);
    var createdQualification = await _qualificationRepository.CreateAsync(qualification);

    var qualificationResponseDto = _mapper.Map<QualificationResponseDto>(createdQualification);
    return CreatedAtAction(nameof(GetQualification), new { id = createdQualification.Id }, qualificationResponseDto);
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "AdminPolicy")]
  public async Task<ActionResult<QualificationResponseDto>> UpdateQualification(Guid id, UpdateQualificationDto updateQualificationDto)
  {
    var validationResult = await _updateQualificationValidator.ValidateAsync(updateQualificationDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var existingQualification = await _qualificationRepository.GetByIdAsync(id);
    if (existingQualification == null)
    {
      return NotFound($"The qualification with ID {id} could not be found");
    }

    if (await _qualificationRepository.ExistsByNameAsync(updateQualificationDto.QualificationName, id))
    {
      return BadRequest($"The qualification '{updateQualificationDto.QualificationName}' already exists. Please use a different name");
    }

    _mapper.Map(updateQualificationDto, existingQualification);
    var updatedQualification = await _qualificationRepository.UpdateAsync(existingQualification);

    var qualificationResponseDto = _mapper.Map<QualificationResponseDto>(updatedQualification);
    return Ok(qualificationResponseDto);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminPolicy")]
  public async Task<IActionResult> DeleteQualification(Guid id)
  {
    var qualification = await _qualificationRepository.GetByIdAsync(id);
    if (qualification == null)
    {
      return NotFound($"The qualification with ID {id} could not be found");
    }

    if (await _qualificationRepository.IsInUseAsync(id))
    {
      return BadRequest("This qualification cannot be deleted as it is currently being used by jobs or candidates. Please remove it from those entities first");
    }

    await _qualificationRepository.DeleteAsync(id);
    return NoContent();
  }

  [HttpGet("count")]
  public async Task<ActionResult<int>> GetQualificationsCount([FromQuery] string? search = null)
  {
    var count = await _qualificationRepository.GetCountAsync(search);
    return Ok(count);
  }

  [HttpGet("exists/{qualificationName}")]
  public async Task<ActionResult<bool>> CheckQualificationExists(string qualificationName)
  {
    var exists = await _qualificationRepository.ExistsByNameAsync(qualificationName);
    return Ok(exists);
  }

  [HttpGet("statistics")]
  public async Task<ActionResult<object>> GetQualificationStatistics()
  {
    var totalCount = await _qualificationRepository.GetCountAsync();
    var inUseCount = await _qualificationRepository.GetInUseCountAsync();
    var availableCount = totalCount - inUseCount;

    var statistics = new
    {
      TotalQualifications = totalCount,
      InUseQualifications = inUseCount,
      AvailableQualifications = availableCount,
      UsagePercentage = totalCount > 0 ? Math.Round((double)inUseCount / totalCount * 100, 2) : 0
    };

    return Ok(statistics);
  }
}
