using Backend.Dtos.Roles;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using AutoMapper;

namespace Backend.Services;

public class RoleService : IRoleService
{
  private readonly IRoleRepository _repo;
  private readonly IMapper _mapper;

  public RoleService(IRoleRepository repo, IMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<RoleDto?> GetById(Guid id)
  {
    var role = await _repo.GetById(id);
    return role == null ? null : _mapper.Map<RoleDto>(role);
  }

  public async Task<List<RoleDto>> GetAll()
  {
    var roles = await _repo.GetAll();
    return _mapper.Map<List<RoleDto>>(roles);
  }

  public async Task<RoleDto> Create(RoleCreateDto dto)
  {
    var role = _mapper.Map<Role>(dto);
    role.Id = Guid.NewGuid();

    await _repo.Add(role);
    return _mapper.Map<RoleDto>(role);
  }

  public async Task<RoleDto?> Update(Guid id, RoleUpdateDto dto)
  {
    var role = await _repo.GetById(id);
    if (role == null) return null;

    _mapper.Map(dto, role);
    await _repo.Update(role);

    return _mapper.Map<RoleDto>(role);
  }

  public async Task<bool> Delete(Guid id)
  {
    await _repo.DeleteById(id);
    return true;
  }
}
