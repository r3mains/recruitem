using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using backend.Repositories.IRepositories;
using backend.DTOs.Skill;
using backend.Models;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("skills")]
[Authorize]
public class SkillController(ISkillRepository skillRepository, IMapper mapper) : ControllerBase
{
  private readonly ISkillRepository _skillRepository = skillRepository;
  private readonly IMapper _mapper = mapper;

  [HttpGet]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetSkills([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
  {
    try
    {
      var skills = await _skillRepository.GetSkillsAsync(page, pageSize, search);
      var totalCount = await _skillRepository.GetSkillCountAsync();

      return Ok(new
      {
        Skills = skills,
        Pagination = new
        {
          Page = page,
          PageSize = pageSize,
          TotalCount = totalCount,
          TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        }
      });
    }
    catch (Exception ex)
    {
      return BadRequest($"Error retrieving skills: {ex.Message}");
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ViewJobs")]
  public async Task<IActionResult> GetSkill(Guid id)
  {
    try
    {
      var skill = await _skillRepository.GetSkillDetailsByIdAsync(id);
      if (skill == null)
      {
        return NotFound("Skill not found");
      }

      return Ok(skill);
    }
    catch (Exception ex)
    {
      return BadRequest($"Error retrieving skill: {ex.Message}");
    }
  }

  [HttpPost]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto createSkillDto)
  {
    try
    {
      if (await _skillRepository.SkillNameExistsAsync(createSkillDto.SkillName))
      {
        return BadRequest("Skill with this name already exists");
      }

      var skill = _mapper.Map<Skill>(createSkillDto);
      var createdSkill = await _skillRepository.CreateSkillAsync(skill);
      var skillDetails = await _skillRepository.GetSkillDetailsByIdAsync(createdSkill.Id);

      return CreatedAtAction(nameof(GetSkill), new { id = createdSkill.Id }, skillDetails);
    }
    catch (Exception ex)
    {
      return BadRequest($"Error creating skill: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> UpdateSkill(Guid id, [FromBody] UpdateSkillDto updateSkillDto)
  {
    try
    {
      var existingSkill = await _skillRepository.GetSkillByIdAsync(id);
      if (existingSkill == null)
      {
        return NotFound("Skill not found");
      }

      if (!string.IsNullOrEmpty(updateSkillDto.SkillName) &&
          await _skillRepository.SkillNameExistsAsync(updateSkillDto.SkillName, id))
      {
        return BadRequest("Skill with this name already exists");
      }

      if (!string.IsNullOrEmpty(updateSkillDto.SkillName))
        existingSkill.SkillName = updateSkillDto.SkillName;

      var updatedSkill = await _skillRepository.UpdateSkillAsync(existingSkill);
      var skillDetails = await _skillRepository.GetSkillDetailsByIdAsync(id);

      return Ok(skillDetails);
    }
    catch (Exception ex)
    {
      return BadRequest($"Error updating skill: {ex.Message}");
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageJobs")]
  public async Task<IActionResult> DeleteSkill(Guid id)
  {
    try
    {
      if (!await _skillRepository.ExistsAsync(id))
      {
        return NotFound("Skill not found");
      }

      var skillDetails = await _skillRepository.GetSkillDetailsByIdAsync(id);
      if (skillDetails != null && (skillDetails.PositionCount > 0 || skillDetails.JobCount > 0 || skillDetails.CandidateCount > 0))
      {
        return BadRequest("Cannot delete skill that is currently being used in positions, jobs, or candidate profiles");
      }

      await _skillRepository.DeleteSkillAsync(id);
      return NoContent();
    }
    catch (Exception ex)
    {
      return BadRequest($"Error deleting skill: {ex.Message}");
    }
  }
}
