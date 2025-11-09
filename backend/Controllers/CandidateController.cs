using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Candidate;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Validators.CandidateValidators;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "UserPolicy")]
public class CandidateController : ControllerBase
{
  private readonly ICandidateRepository _candidateRepository;
  private readonly IMapper _mapper;
  private readonly UserManager<User> _userManager;
  private readonly CreateCandidateValidator _createCandidateValidator;
  private readonly UpdateCandidateValidator _updateCandidateValidator;

  public CandidateController(
    ICandidateRepository candidateRepository,
    IMapper mapper,
    UserManager<User> userManager,
    CreateCandidateValidator createCandidateValidator,
    UpdateCandidateValidator updateCandidateValidator)
  {
    _candidateRepository = candidateRepository;
    _mapper = mapper;
    _userManager = userManager;
    _createCandidateValidator = createCandidateValidator;
    _updateCandidateValidator = updateCandidateValidator;
  }

  // GET /api/v1/candidate
  [HttpGet]
  [Authorize(Policy = "ViewCandidates")]
  public async Task<ActionResult<IEnumerable<CandidateListDto>>> GetCandidates(
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
  {
    if (page < 1 || pageSize < 1 || pageSize > 100)
    {
      return BadRequest("Page must be >= 1 and PageSize must be between 1 and 100");
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
  public async Task<ActionResult<CandidateResponseDto>> GetCandidate(Guid id)
  {
    var candidate = await _candidateRepository.GetByIdAsync(id);
    if (candidate == null)
    {
      return NotFound($"Candidate with ID {id} not found");
    }

    var candidateResponseDto = _mapper.Map<CandidateResponseDto>(candidate);
    return Ok(candidateResponseDto);
  }

  [HttpPost]
  [Authorize(Policy = "ManageCandidates")]
  public async Task<ActionResult<CandidateResponseDto>> CreateCandidate(CreateCandidateDto createCandidateDto)
  {
    var validationResult = await _createCandidateValidator.ValidateAsync(createCandidateDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    if (await _candidateRepository.ExistsByEmailAsync(createCandidateDto.Email))
    {
      return BadRequest($"A candidate with email '{createCandidateDto.Email}' already exists");
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
      return BadRequest(userResult.Errors);
    }

    await _userManager.AddToRoleAsync(user, "Candidate");

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
  public async Task<ActionResult<CandidateResponseDto>> UpdateCandidate(Guid id, UpdateCandidateDto updateCandidateDto)
  {
    var validationResult = await _updateCandidateValidator.ValidateAsync(updateCandidateDto);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var existingCandidate = await _candidateRepository.GetByIdAsync(id);
    if (existingCandidate == null)
    {
      return NotFound($"Candidate with ID {id} not found");
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
  public async Task<ActionResult<CandidateResponseDto>> UpdateMyProfile(UpdateCandidateDto updateCandidateDto)
  {
    var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value!);
    var candidate = await _candidateRepository.GetByUserIdAsync(userId);

    if (candidate == null)
    {
      return BadRequest("Candidate profile not found");
    }

    return await UpdateCandidate(candidate.Id, updateCandidateDto);
  }

  // GET /api/v1/candidate/profile
  [HttpGet("profile")]
  [Authorize(Roles = "Candidate")]
  public async Task<ActionResult<CandidateResponseDto>> GetMyProfile()
  {
    var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value!);
    var candidate = await _candidateRepository.GetByUserIdAsync(userId);

    if (candidate == null)
    {
      return NotFound("Candidate profile not found");
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
      return NotFound($"Candidate with ID {id} not found");
    }

    await _candidateRepository.DeleteAsync(id);
    return NoContent();
  }
}
