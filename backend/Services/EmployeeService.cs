using Backend.Dtos.Employees;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class EmployeeService : IEmployeeService
{
  private readonly IEmployeeRepository _repo;
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public EmployeeService(IEmployeeRepository repo, AppDbContext context, IMapper mapper)
  {
    _repo = repo;
    _context = context;
    _mapper = mapper;
  }

  public async Task<EmployeeDto?> GetById(Guid id)
  {
    var employee = await _context.Employees
        .Include(e => e.User)
            .ThenInclude(u => u!.Role)
        .Include(e => e.BranchAddress)
            .ThenInclude(a => a!.City)
        .FirstOrDefaultAsync(e => e.Id == id);

    return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
  }

  public async Task<List<EmployeeDto>> GetAll()
  {
    var employees = await _context.Employees
        .Include(e => e.User)
            .ThenInclude(u => u!.Role)
        .Include(e => e.BranchAddress)
            .ThenInclude(a => a!.City)
        .ToListAsync();

    return _mapper.Map<List<EmployeeDto>>(employees);
  }

  public async Task<EmployeeDto> Create(EmployeeCreateDto dto)
  {
    var employee = _mapper.Map<Employee>(dto);
    employee.Id = Guid.NewGuid();

    await _repo.Add(employee);
    return await GetById(employee.Id) ?? throw new Exception("Failed to create employee");
  }

  public async Task<EmployeeDto?> Update(Guid id, EmployeeUpdateDto dto)
  {
    var employee = await _repo.GetById(id);
    if (employee == null) return null;

    _mapper.Map(dto, employee);
    await _repo.Update(employee);

    return await GetById(id);
  }

  public async Task<bool> Delete(Guid id)
  {
    await _repo.DeleteById(id);
    return true;
  }
}
