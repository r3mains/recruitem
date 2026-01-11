using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
  public class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
      return await _context.Employees
          .Include(e => e.User)
          .Include(e => e.BranchAddress)
              .ThenInclude(a => a!.City)
                  .ThenInclude(c => c!.State)
                      .ThenInclude(s => s!.Country)
          .Where(e => !e.IsDeleted)
          .ToListAsync();
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
