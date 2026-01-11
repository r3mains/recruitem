using AutoMapper;
using backend.DTOs.JobType;
using backend.Repositories.IRepositories;
using backend.Validators.JobTypeValidators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class JobTypeController(
  IJobTypeRepository jobTypeRepository,
  IMapper mapper,
  CreateJobTypeValidator createJobTypeValidator,
  UpdateJobTypeValidator updateJobTypeValidator) : ControllerBase
{
  private readonly IJobTypeRepository _jobTypeRepository = jobTypeRepository;
  private readonly IMapper _mapper = mapper;
  private readonly CreateJobTypeValidator _createJobTypeValidator = createJobTypeValidator;
  private readonly UpdateJobTypeValidator _updateJobTypeValidator = updateJobTypeValidator;

  [HttpGet]
  [AllowAnonymous]
  public async Task<ActionResult<IEnumerable<JobTypeListDto>>> GetJobTypes(
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Invalid pagination parameters. Page must be 1 or greater, and page size must be between 1 and 100");
    }

    var jobTypes = await _jobTypeRepository.GetAllAsync(search, page, pageSize);
    var jobTypeListDtos = _mapper.Map<IEnumerable<JobTypeListDto>>(jobTypes);

    return Ok(jobTypeListDtos);
  }

  [HttpGet("{id}")]
  [AllowAnonymous]
  public async Task<ActionResult<JobTypeResponseDto>> GetJobType(Guid id)
  {
    var jobType = await _jobTypeRepository.GetByIdAsync(id);
    if (jobType == null)
    {
      return NotFound($"The job type with ID {id} could not be found");
    }

    var jobTypeResponseDto = _mapper.Map<JobTypeResponseDto>(jobType);
    return Ok(jobTypeResponseDto);
  }

  [HttpPost]
  [Authorize(Policy = "AdminPolicy")]
  public async Task<ActionResult<JobTypeResponseDto>> CreateJobType(CreateJobTypeDto createJobTypeDto)
  {
    var validationResult = await _createJobTypeValidator.ValidateAsync(createJobTypeDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    if (await _jobTypeRepository.ExistsByNameAsync(createJobTypeDto.Type))
    {
      return BadRequest($"The job type '{createJobTypeDto.Type}' already exists. Please use a different name");
    }

    var jobType = _mapper.Map<backend.Models.JobType>(createJobTypeDto);
    var createdJobType = await _jobTypeRepository.CreateAsync(jobType);

    var jobTypeResponseDto = _mapper.Map<JobTypeResponseDto>(createdJobType);
    return CreatedAtAction(nameof(GetJobType), new { id = createdJobType.Id }, jobTypeResponseDto);
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "AdminPolicy")]
  public async Task<ActionResult<JobTypeResponseDto>> UpdateJobType(Guid id, UpdateJobTypeDto updateJobTypeDto)
  {
    var validationResult = await _updateJobTypeValidator.ValidateAsync(updateJobTypeDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var existingJobType = await _jobTypeRepository.GetByIdAsync(id);
    if (existingJobType == null)
    {
      return NotFound($"The job type with ID {id} could not be found");
    }

    if (await _jobTypeRepository.ExistsByNameAsync(updateJobTypeDto.Type, id))
    {
      return BadRequest($"The job type '{updateJobTypeDto.Type}' already exists. Please use a different name");
    }

    _mapper.Map(updateJobTypeDto, existingJobType);
    var updatedJobType = await _jobTypeRepository.UpdateAsync(existingJobType);

    var jobTypeResponseDto = _mapper.Map<JobTypeResponseDto>(updatedJobType);
    return Ok(jobTypeResponseDto);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminPolicy")]
  public async Task<IActionResult> DeleteJobType(Guid id)
  {
    var jobType = await _jobTypeRepository.GetByIdAsync(id);
    if (jobType == null)
    {
      return NotFound($"The job type with ID {id} could not be found");
    }

    if (await _jobTypeRepository.IsInUseAsync(id))
    {
      return BadRequest("This job type cannot be deleted as it is currently being used by jobs. Please remove it from those jobs first");
    }

    await _jobTypeRepository.DeleteAsync(id);
    return NoContent();
  }

  [HttpGet("count")]
  public async Task<ActionResult<int>> GetJobTypesCount([FromQuery] string? search = null)
  {
    var count = await _jobTypeRepository.GetCountAsync(search);
    return Ok(count);
  }

  [HttpGet("exists/{type}")]
  public async Task<ActionResult<bool>> CheckJobTypeExists(string type)
  {
    var exists = await _jobTypeRepository.ExistsByNameAsync(type);
    return Ok(exists);
  }
}
