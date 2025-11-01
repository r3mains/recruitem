using Backend.Dtos.Users;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class UserService : IUserService
{
  private readonly IUserRepository _repo;
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public UserService(IUserRepository repo, AppDbContext context, IMapper mapper)
  {
    _repo = repo;
    _context = context;
    _mapper = mapper;
  }

  public async Task<UserDto?> GetById(Guid id)
  {
    var user = await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Id == id);

    return user == null ? null : _mapper.Map<UserDto>(user);
  }

  public async Task<List<UserDto>> GetAll()
  {
    var users = await _context.Users
        .Include(u => u.Role)
        .ToListAsync();

    return _mapper.Map<List<UserDto>>(users);
  }

  public async Task<UserDto> Create(UserCreateDto dto)
  {
    var user = _mapper.Map<User>(dto);
    user.Id = Guid.NewGuid();

    await _repo.Add(user);
    return await GetById(user.Id) ?? throw new Exception("Failed to create user");
  }

  public async Task<UserDto?> Update(Guid id, UserUpdateDto dto)
  {
    var user = await _repo.GetById(id);
    if (user == null) return null;

    _mapper.Map(dto, user);
    await _repo.Update(user);

    return await GetById(id);
  }

  public async Task<bool> Delete(Guid id)
  {
    await _repo.DeleteById(id);
    return true;
  }
}
