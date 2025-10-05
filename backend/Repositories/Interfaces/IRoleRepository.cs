using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IRoleRepository
{
  Task<Role?> GetById(Guid id);
  Task<List<Role>> GetAll();
  Task Add(Role entity);
  Task Update(Role entity);
  Task DeleteById(Guid id);
}
