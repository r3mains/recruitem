using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IUserRepository
{
  Task<User?> GetById(Guid id);
  Task<List<User>> GetAll();
  Task Add(User entity);
  Task Update(User entity);
  Task DeleteById(Guid id);
}
