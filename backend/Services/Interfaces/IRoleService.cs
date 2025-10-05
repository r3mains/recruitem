using Backend.Dtos.Roles;

namespace Backend.Services.Interfaces;

public interface IRoleService
{
  Task<RoleDto?> GetById(Guid id);
  Task<List<RoleDto>> GetAll();
  Task<RoleDto> Create(RoleCreateDto dto);
  Task<RoleDto?> Update(Guid id, RoleUpdateDto dto);
  Task<bool> Delete(Guid id);
}
