using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IEmployeeRepository
{
  Task<Employee?> GetById(Guid id);
  Task<List<Employee>> GetAll();
  Task Add(Employee entity);
  Task Update(Employee entity);
  Task DeleteById(Guid id);
}
