using backend.Data;
using backend.DTOs.EmailTemplate;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class EmailTemplateRepository(ApplicationDbContext context) : IEmailTemplateRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<EmailTemplateDto?> GetByIdAsync(Guid id)
  {
    var template = await _context.EmailTemplates.FindAsync(id);
    return template != null ? MapToDto(template) : null;
  }

  public async Task<List<EmailTemplateDto>> GetAllAsync()
  {
    var templates = await _context.EmailTemplates
      .OrderBy(t => t.Category)
      .ThenBy(t => t.Name)
      .ToListAsync();

    return templates.Select(MapToDto).ToList();
  }

  public async Task<List<EmailTemplateDto>> GetByCategoryAsync(string category)
  {
    var templates = await _context.EmailTemplates
      .Where(t => t.Category == category)
      .OrderBy(t => t.Name)
      .ToListAsync();

    return templates.Select(MapToDto).ToList();
  }

  public async Task<List<EmailTemplateDto>> GetActiveTemplatesAsync()
  {
    var templates = await _context.EmailTemplates
      .Where(t => t.IsActive)
      .OrderBy(t => t.Category)
      .ThenBy(t => t.Name)
      .ToListAsync();

    return templates.Select(MapToDto).ToList();
  }

  public async Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto dto)
  {
    var template = new EmailTemplate
    {
      Id = Guid.NewGuid(),
      Name = dto.Name,
      Subject = dto.Subject,
      Body = dto.Body,
      Description = dto.Description,
      Category = dto.Category,
      AvailableVariables = dto.AvailableVariables,
      IsActive = dto.IsActive,
      CreatedAt = DateTime.UtcNow
    };

    _context.EmailTemplates.Add(template);
    await _context.SaveChangesAsync();

    return MapToDto(template);
  }

  public async Task<EmailTemplateDto?> UpdateAsync(Guid id, UpdateEmailTemplateDto dto)
  {
    var template = await _context.EmailTemplates.FindAsync(id);
    if (template == null) return null;

    if (dto.Name != null) template.Name = dto.Name;
    if (dto.Subject != null) template.Subject = dto.Subject;
    if (dto.Body != null) template.Body = dto.Body;
    if (dto.Description != null) template.Description = dto.Description;
    if (dto.Category != null) template.Category = dto.Category;
    if (dto.AvailableVariables != null) template.AvailableVariables = dto.AvailableVariables;
    if (dto.IsActive.HasValue) template.IsActive = dto.IsActive.Value;
    
    template.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return MapToDto(template);
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var template = await _context.EmailTemplates.FindAsync(id);
    if (template == null) return false;

    _context.EmailTemplates.Remove(template);
    await _context.SaveChangesAsync();

    return true;
  }

  public async Task<bool> ExistsAsync(Guid id)
  {
    return await _context.EmailTemplates.AnyAsync(t => t.Id == id);
  }

  public async Task<bool> ExistsByNameAsync(string name)
  {
    return await _context.EmailTemplates.AnyAsync(t => t.Name == name);
  }

  private static EmailTemplateDto MapToDto(EmailTemplate template)
  {
    return new EmailTemplateDto(
      template.Id,
      template.Name,
      template.Subject,
      template.Body,
      template.Description,
      template.Category,
      template.AvailableVariables,
      template.IsActive,
      template.CreatedAt,
      template.UpdatedAt
    );
  }
}
