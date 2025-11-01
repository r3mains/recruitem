using Backend.Dtos.Positions;

namespace Backend.Services.Interfaces;

public interface IPositionService
{
  Task<PositionDto?> GetById(Guid id);
  Task<List<PositionDto>> GetAll();
  Task<PositionDto> Create(PositionCreateDto dto);
  Task<PositionDto?> Update(Guid id, PositionUpdateDto dto);
  Task<bool> Delete(Guid id);
  Task<PositionDto?> UpdateStatus(Guid id, PositionStatusUpdateDto dto);
}
