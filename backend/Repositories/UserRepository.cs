using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
  private readonly AppDbContext _db = db;

  public async Task<User?> GetById(Guid id)
  {
    return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<List<User>> GetAll()
  {
    return await _db.Users.ToListAsync();
  }

  public async Task Add(User entity)
  {
    await _db.Users.AddAsync(entity);
    await _db.SaveChangesAsync();
  }

  public async Task Update(User entity)
  {
    _db.Users.Update(entity);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteById(Guid id)
  {
    var entity = await _db.Users.FindAsync(id);
    if (entity != null)
    {
      _db.Users.Remove(entity);
      await _db.SaveChangesAsync();
    }
  }
}
