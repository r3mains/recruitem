using Backend.Dtos.Users;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class UserService(IUserRepository repo) : IUserService
{
  public async Task<UserDto?> GetById(Guid id)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    return new UserDto { Id = e.Id, Email = e.Email, RoleId = e.RoleId, CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt };
  }

  public async Task<List<UserDto>> GetAll()
  {
    var list = await repo.GetAll();
    return list.Select(e => new UserDto { Id = e.Id, Email = e.Email, RoleId = e.RoleId, CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt }).ToList();
  }

  public async Task<UserDto> Create(UserCreateDto dto)
  {
    var e = new User { Id = Guid.NewGuid(), Email = dto.Email, Password = dto.Password, RoleId = dto.RoleId, CreatedAt = DateTime.UtcNow };
    await repo.Add(e);
    return new UserDto { Id = e.Id, Email = e.Email, RoleId = e.RoleId, CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt };
  }

  public async Task<UserDto?> Update(Guid id, UserUpdateDto dto)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    if (!string.IsNullOrWhiteSpace(dto.Email)) e.Email = dto.Email;
    if (!string.IsNullOrWhiteSpace(dto.Password)) e.Password = dto.Password;
    if (dto.RoleId.HasValue) e.RoleId = dto.RoleId.Value;
    e.UpdatedAt = DateTime.UtcNow;
    await repo.Update(e);
    return new UserDto { Id = e.Id, Email = e.Email, RoleId = e.RoleId, CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt };
  }

  public async Task<bool> Delete(Guid id)
  {
    await repo.DeleteById(id);
    return true;
  }
}
