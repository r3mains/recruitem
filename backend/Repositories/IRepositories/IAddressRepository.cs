using backend.Models;

namespace backend.Repositories.IRepositories
{
  public interface IAddressRepository
  {
    Task<IEnumerable<Address>> GetAllAddressesAsync();
    Task<Address?> GetAddressByIdAsync(Guid id);
  }
}
