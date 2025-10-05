using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class CandidateRepository(AppDbContext db) : ICandidateRepository
{
  private readonly AppDbContext _db = db;

  public async Task<Candidate?> GetById(Guid id)
  {
    return await _db.Candidates.FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<List<Candidate>> GetAll()
  {
    return await _db.Candidates.ToListAsync();
  }

  public async Task Add(Candidate entity)
  {
    await _db.Candidates.AddAsync(entity);
    await _db.SaveChangesAsync();
  }

  public async Task Update(Candidate entity)
  {
    _db.Candidates.Update(entity);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteById(Guid id)
  {
    var entity = await _db.Candidates.FindAsync(id);
    if (entity != null)
    {
      _db.Candidates.Remove(entity);
      await _db.SaveChangesAsync();
    }
  }
}
