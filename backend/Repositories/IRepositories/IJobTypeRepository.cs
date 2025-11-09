namespace backend.Repositories.IRepositories;

public interface IJobTypeRepository
{
  Task<IEnumerable<backend.Models.JobType>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10);
  Task<backend.Models.JobType?> GetByIdAsync(Guid id);
  Task<backend.Models.JobType> CreateAsync(backend.Models.JobType jobType);
  Task<backend.Models.JobType> UpdateAsync(backend.Models.JobType jobType);
  Task DeleteAsync(Guid id);
  Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
  Task<bool> IsInUseAsync(Guid id);
  Task<int> GetCountAsync(string? search = null);
}
