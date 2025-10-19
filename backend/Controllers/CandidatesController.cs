using Backend.Dtos.Candidates;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/candidates")]
[Authorize]
public class CandidatesController(ICandidateService service) : BaseController
{
  [HttpGet]
  [Authorize]
  public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? skills, [FromQuery] int page = 1, [FromQuery] int limit = 20)
  {
    var candidates = await service.Search(search, skills, page, limit);
    return Ok(candidates);
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<IActionResult> GetById(Guid id)
  {
    var candidate = await service.GetById(id);
    return NotFoundIfNull(candidate);
  }

  [HttpGet("profile")]
  [Authorize]
  public async Task<IActionResult> GetMyProfile()
  {
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    var candidate = await service.GetByUserId(userId.Value);
    return NotFoundIfNull(candidate, "Candidate profile not found");
  }

  [HttpPost]
  public async Task<IActionResult> Create(CandidateCreateDto dto)
  {
    var userId = GetCurrentUserId();
    if (userId != null)
    {
      dto.UserId = userId.Value;
    }
    var r = await service.Create(dto);
    return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, CandidateUpdateDto dto)
  {
    var userId = GetCurrentUserId();
    var userRole = GetCurrentUserRole();

    if (userRole == "Candidate")
    {
      var candidate = await service.GetById(id);
      if (candidate?.UserId != userId)
        return Forbid("You can only update your own profile");
    }

    var result = await service.Update(id, dto);
    return NotFoundIfNull(result);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var success = await service.Delete(id);
    if (!success) return NotFound();
    return NoContent();
  }

  [HttpPost("register")]
  [AllowAnonymous]
  public async Task<IActionResult> RegisterCandidate([FromBody] CreateCandidateDto dto)
  {
    try
    {
      var candidate = await service.CreateCandidateProfile(dto);
      return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, candidate);
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateCandidateDto dto)
  {
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    var candidate = await service.GetByUserId(userId.Value);
    if (candidate == null) return NotFound();

    var updated = await service.UpdateCandidateProfile(candidate.Id, dto);
    return NotFoundIfNull(updated);
  }

  [HttpPost("search")]
  [Authorize(Policy = "StaffOnly")]
  public async Task<IActionResult> SearchCandidates([FromBody] CandidateSearchDto searchDto)
  {
    var candidates = await service.SearchCandidates(searchDto);
    return Ok(candidates);
  }

  [HttpGet("by-skills")]
  [Authorize(Policy = "StaffOnly")]
  public async Task<IActionResult> GetCandidatesBySkills([FromQuery] List<Guid> skillIds, [FromQuery] int? minExperience)
  {
    var candidates = await service.GetCandidatesBySkills(skillIds, minExperience);
    return Ok(candidates);
  }

  [HttpGet("{candidateId}/skills")]
  [Authorize]
  public async Task<IActionResult> GetCandidateSkills(Guid candidateId)
  {
    var userId = GetCurrentUserId();
    var userRole = GetCurrentUserRole();

    if (userRole == "Candidate")
    {
      var candidate = await service.GetById(candidateId);
      if (candidate?.UserId != userId)
        return Forbid("You can only access your own skills");
    }

    var skills = await service.GetCandidateSkills(candidateId);
    return Ok(skills);
  }

  [HttpPost("{candidateId}/skills")]
  [Authorize]
  public async Task<IActionResult> AddCandidateSkill(Guid candidateId, [FromBody] CandidateSkillCreateDto dto)
  {
    var userId = GetCurrentUserId();
    var userRole = GetCurrentUserRole();

    if (userRole == "Candidate")
    {
      var candidate = await service.GetById(candidateId);
      if (candidate?.UserId != userId)
        return Forbid("You can only add skills to your own profile");
    }

    var skill = await service.AddCandidateSkill(candidateId, dto);
    return Ok(skill);
  }

  [HttpPut("{candidateId}/skills/{skillId}")]
  [Authorize]
  public async Task<IActionResult> UpdateCandidateSkill(Guid candidateId, Guid skillId, [FromBody] CandidateSkillUpdateDto dto)
  {
    var userId = GetCurrentUserId();
    var userRole = GetCurrentUserRole();

    if (userRole == "Candidate")
    {
      var candidate = await service.GetById(candidateId);
      if (candidate?.UserId != userId)
        return Forbid("You can only update your own skills");
    }

    var skill = await service.UpdateCandidateSkill(candidateId, skillId, dto);
    return NotFoundIfNull(skill);
  }

  [HttpDelete("{candidateId}/skills/{skillId}")]
  [Authorize]
  public async Task<IActionResult> RemoveCandidateSkill(Guid candidateId, Guid skillId)
  {
    var userId = GetCurrentUserId();
    var userRole = GetCurrentUserRole();

    if (userRole == "Candidate")
    {
      var candidate = await service.GetById(candidateId);
      if (candidate?.UserId != userId)
        return Forbid("You can only remove your own skills");
    }

    var success = await service.RemoveCandidateSkill(candidateId, skillId);
    if (!success) return NotFound();
    return NoContent();
  }
}
