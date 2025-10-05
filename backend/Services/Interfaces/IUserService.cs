using Backend.Dtos.Users;

namespace Backend.Services.Interfaces;

public interface IUserService
{
  Task<UserDto?> GetById(Guid id);
  Task<List<UserDto>> GetAll();
  Task<UserDto> Create(UserCreateDto dto);
  Task<UserDto?> Update(Guid id, UserUpdateDto dto);
  Task<bool> Delete(Guid id);
}
