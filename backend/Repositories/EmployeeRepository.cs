using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
  private readonly AppDbContext _db = db;

  public async Task<Employee?> GetById(Guid id)
  {
    return await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<List<Employee>> GetAll()
  {
    return await _db.Employees.ToListAsync();
  }

  public async Task Add(Employee entity)
  {
    await _db.Employees.AddAsync(entity);
    await _db.SaveChangesAsync();
  }

  public async Task Update(Employee entity)
  {
    _db.Employees.Update(entity);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteById(Guid id)
  {
    var entity = await _db.Employees.FindAsync(id);
    if (entity != null)
    {
      _db.Employees.Remove(entity);
      await _db.SaveChangesAsync();
    }
  }
}
