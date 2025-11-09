using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
  public class EmployeeRepository : IEmployeeRepository
  {
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId)
    {
      return await _context.Employees
          .Include(e => e.User)
          .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
    {
      return await _context.Employees
          .Include(e => e.User)
          .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }
  }
}
