using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class RoleRepository(AppDbContext db) : IRoleRepository
{
  private readonly AppDbContext _db = db;

  public async Task<Role?> GetById(Guid id)
  {
    return await _db.Roles.FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<List<Role>> GetAll()
  {
    return await _db.Roles.ToListAsync();
  }

  public async Task Add(Role entity)
  {
    await _db.Roles.AddAsync(entity);
    await _db.SaveChangesAsync();
  }

  public async Task Update(Role entity)
  {
    _db.Roles.Update(entity);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteById(Guid id)
  {
    var entity = await _db.Roles.FindAsync(id);
    if (entity != null)
    {
      _db.Roles.Remove(entity);
      await _db.SaveChangesAsync();
    }
  }
}
