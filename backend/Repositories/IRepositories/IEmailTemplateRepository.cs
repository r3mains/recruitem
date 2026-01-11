using backend.DTOs.EmailTemplate;

namespace backend.Repositories.IRepositories;

public interface IEmailTemplateRepository
{
  Task<EmailTemplateDto?> GetByIdAsync(Guid id);
  Task<List<EmailTemplateDto>> GetAllAsync();
  Task<List<EmailTemplateDto>> GetByCategoryAsync(string category);
  Task<List<EmailTemplateDto>> GetActiveTemplatesAsync();
  Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto dto);
  Task<EmailTemplateDto?> UpdateAsync(Guid id, UpdateEmailTemplateDto dto);
  Task<bool> DeleteAsync(Guid id);
  Task<bool> ExistsAsync(Guid id);
  Task<bool> ExistsByNameAsync(string name);
}
