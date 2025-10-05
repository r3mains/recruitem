using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IJobRepository
{
  Task<Job?> GetById(Guid id);
  Task<List<Job>> GetAll();
  Task Add(Job entity);
  Task Update(Job entity);
  Task DeleteById(Guid id);
}
