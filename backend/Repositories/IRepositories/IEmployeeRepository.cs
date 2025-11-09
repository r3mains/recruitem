using backend.Models;

namespace backend.Repositories.IRepositories
{
  public interface IEmployeeRepository
  {
    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId);
    Task<Employee?> GetEmployeeByIdAsync(Guid id);
  }
}
