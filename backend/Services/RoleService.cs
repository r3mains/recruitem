using Backend.Dtos.Roles;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class RoleService(IRoleRepository repo) : IRoleService
{
  public async Task<RoleDto?> GetById(Guid id)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    return new RoleDto { Id = e.Id, Name = e.Name };
  }

  public async Task<List<RoleDto>> GetAll()
  {
    var list = await repo.GetAll();
    return list.Select(e => new RoleDto { Id = e.Id, Name = e.Name }).ToList();
  }

  public async Task<RoleDto> Create(RoleCreateDto dto)
  {
    var e = new Role { Id = Guid.NewGuid(), Name = dto.Name };
    await repo.Add(e);
    return new RoleDto { Id = e.Id, Name = e.Name };
  }

  public async Task<RoleDto?> Update(Guid id, RoleUpdateDto dto)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    if (!string.IsNullOrWhiteSpace(dto.Name)) e.Name = dto.Name;
    await repo.Update(e);
    return new RoleDto { Id = e.Id, Name = e.Name };
  }

  public async Task<bool> Delete(Guid id)
  {
    await repo.DeleteById(id);
    return true;
  }
}
