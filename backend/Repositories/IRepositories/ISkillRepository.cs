using backend.Models;
using backend.DTOs.Skill;

namespace backend.Repositories.IRepositories
{
  public interface ISkillRepository
  {
    Task<Skill> CreateSkillAsync(Skill skill);
    Task<Skill?> GetSkillByIdAsync(Guid id);
    Task<SkillResponseDto?> GetSkillDetailsByIdAsync(Guid id);
    Task<IEnumerable<SkillListDto>> GetSkillsAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<Skill> UpdateSkillAsync(Skill skill);
    Task DeleteSkillAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetSkillCountAsync();
    Task<bool> SkillNameExistsAsync(string skillName, Guid? excludeId = null);
  }
}
