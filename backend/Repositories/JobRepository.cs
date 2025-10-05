using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class JobRepository(AppDbContext db) : IJobRepository
{
  public async Task<Job?> GetById(Guid id) => await db.Jobs.FindAsync(id);
  public async Task<List<Job>> GetAll() => await db.Jobs.ToListAsync();
  public async Task Add(Job entity) { db.Jobs.Add(entity); await db.SaveChangesAsync(); }
  public async Task Update(Job entity) { db.Jobs.Update(entity); await db.SaveChangesAsync(); }
  public async Task DeleteById(Guid id)
  {
    var e = await db.Jobs.FindAsync(id);
    if (e != null) { db.Jobs.Remove(e); await db.SaveChangesAsync(); }
  }
}
