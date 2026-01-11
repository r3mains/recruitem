using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using backend.Repositories.IRepositories;
using backend.DTOs.Profile;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class EmployeeController(
  IEmployeeRepository employeeRepository,
  IMapper mapper) : ControllerBase
{
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;
  private readonly IMapper _mapper = mapper;

  // GET /api/v1/employees
  [HttpGet]
  public async Task<IActionResult> GetAllEmployees()
  {
    var employees = await _employeeRepository.GetAllEmployeesAsync();
    var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    return Ok(employeeDtos);
  }

  // GET /api/v1/employees/{id}
  [HttpGet("{id}")]
  public async Task<IActionResult> GetEmployeeById(Guid id)
  {
    var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
    if (employee == null)
    {
      return NotFound(new { message = "Employee not found" });
    }

    var employeeDto = _mapper.Map<EmployeeDto>(employee);
    return Ok(employeeDto);
  }
}
