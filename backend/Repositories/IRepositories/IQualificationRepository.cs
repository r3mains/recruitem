namespace backend.Repositories.IRepositories;

public interface IQualificationRepository
{
  Task<IEnumerable<backend.Models.Qualification>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10);
  Task<backend.Models.Qualification?> GetByIdAsync(Guid id);
  Task<backend.Models.Qualification> CreateAsync(backend.Models.Qualification qualification);
  Task<backend.Models.Qualification> UpdateAsync(backend.Models.Qualification qualification);
  Task DeleteAsync(Guid id);
  Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
  Task<bool> IsInUseAsync(Guid id);
  Task<int> GetCountAsync(string? search = null);
  Task<int> GetInUseCountAsync();
}
