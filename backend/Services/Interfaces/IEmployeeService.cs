using Backend.Dtos.Employees;

namespace Backend.Services.Interfaces;

public interface IEmployeeService
{
  Task<EmployeeDto?> GetById(Guid id);
  Task<List<EmployeeDto>> GetAll();
  Task<EmployeeDto> Create(EmployeeCreateDto dto);
  Task<EmployeeDto?> Update(Guid id, EmployeeUpdateDto dto);
  Task<bool> Delete(Guid id);
}
