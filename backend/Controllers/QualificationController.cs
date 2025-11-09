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
[Authorize(Policy = "UserPolicy")]
public class QualificationController : ControllerBase
{
  private readonly IQualificationRepository _qualificationRepository;
  private readonly IMapper _mapper;
  private readonly CreateQualificationValidator _createQualificationValidator;
  private readonly UpdateQualificationValidator _updateQualificationValidator;

  public QualificationController(
    IQualificationRepository qualificationRepository,
    IMapper mapper,
    CreateQualificationValidator createQualificationValidator,
    UpdateQualificationValidator updateQualificationValidator)
  {
    _qualificationRepository = qualificationRepository;
    _mapper = mapper;
    _createQualificationValidator = createQualificationValidator;
    _updateQualificationValidator = updateQualificationValidator;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<QualificationListDto>>> GetQualifications(
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Page must be >= 1 and PageSize must be between 1 and 100");
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
      return NotFound($"Qualification with ID {id} not found");
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
      return BadRequest($"Qualification '{createQualificationDto.QualificationName}' already exists");
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
      return NotFound($"Qualification with ID {id} not found");
    }

    if (await _qualificationRepository.ExistsByNameAsync(updateQualificationDto.QualificationName, id))
    {
      return BadRequest($"Qualification '{updateQualificationDto.QualificationName}' already exists");
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
      return NotFound($"Qualification with ID {id} not found");
    }

    if (await _qualificationRepository.IsInUseAsync(id))
    {
      return BadRequest("Cannot delete qualification as it is currently in use by jobs or candidates");
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
