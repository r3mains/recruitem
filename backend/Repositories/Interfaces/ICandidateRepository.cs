using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface ICandidateRepository
{
  Task<Candidate?> GetById(Guid id);
  Task<List<Candidate>> GetAll();
  Task Add(Candidate entity);
  Task Update(Candidate entity);
  Task DeleteById(Guid id);
}
