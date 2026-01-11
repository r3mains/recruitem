using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Repositories.IRepositories;

namespace backend.Repositories
{
  public class AddressRepository(ApplicationDbContext context) : IAddressRepository
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Address>> GetAllAddressesAsync()
    {
      return await _context.Addresses
          .Include(a => a.City)
              .ThenInclude(c => c!.State)
                  .ThenInclude(s => s!.Country)
          .ToListAsync();
    }

    public async Task<Address?> GetAddressByIdAsync(Guid id)
    {
      return await _context.Addresses
          .Include(a => a.City)
              .ThenInclude(c => c!.State)
                  .ThenInclude(s => s!.Country)
          .FirstOrDefaultAsync(a => a.Id == id);
    }
  }
}
