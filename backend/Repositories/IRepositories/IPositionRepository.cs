using backend.Models;
using backend.DTOs.Position;

namespace backend.Repositories.IRepositories
{
  public interface IPositionRepository
  {
    Task<Position> CreatePositionAsync(Position position);
    Task<Position?> GetPositionByIdAsync(Guid id);
    Task<PositionResponseDto?> GetPositionDetailsByIdAsync(Guid id);
    Task<IEnumerable<PositionListDto>> GetPositionsAsync(int page = 1, int pageSize = 10, string? search = null, Guid? statusId = null);
    Task<Position> UpdatePositionAsync(Position position);
    Task DeletePositionAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetPositionCountAsync(Guid? statusId = null);
  }
}
