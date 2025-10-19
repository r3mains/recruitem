using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/skills")]
[Authorize]
public class SkillsController : ControllerBase
{
  private readonly AppDbContext _context;

  public SkillsController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] string? search)
  {
    var query = _context.Skills.AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      query = query.Where(s => s.Name.Contains(search));
    }

    var skills = await query
        .Select(s => new
        {
          Id = s.Id,
          Name = s.Name
        })
        .ToListAsync();

    return Ok(skills);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var skill = await _context.Skills
        .Where(s => s.Id == id)
        .Select(s => new
        {
          Id = s.Id,
          Name = s.Name
        })
        .FirstOrDefaultAsync();

    if (skill == null)
      return NotFound();

    return Ok(skill);
  }

  [HttpPost]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<IActionResult> Create(SkillCreateDto dto)
  {
    var existingSkill = await _context.Skills
        .FirstOrDefaultAsync(s => s.Name.ToLower() == dto.Name.ToLower());

    if (existingSkill != null)
      return BadRequest("Skill already exists");

    var skill = new Skill
    {
      Id = Guid.NewGuid(),
      Name = dto.Name
    };

    _context.Skills.Add(skill);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = skill.Id }, new { Id = skill.Id, Name = skill.Name });
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<IActionResult> Update(Guid id, SkillUpdateDto dto)
  {
    var skill = await _context.Skills.FindAsync(id);
    if (skill == null)
      return NotFound();

    skill.Name = dto.Name;
    await _context.SaveChangesAsync();

    return Ok(new { Id = skill.Id, Name = skill.Name });
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var skill = await _context.Skills.FindAsync(id);
    if (skill == null)
      return NotFound();

    _context.Skills.Remove(skill);
    await _context.SaveChangesAsync();

    return NoContent();
  }
}

public class SkillCreateDto
{
  public string Name { get; set; } = string.Empty;
}

public class SkillUpdateDto
{
  public string Name { get; set; } = string.Empty;
}
