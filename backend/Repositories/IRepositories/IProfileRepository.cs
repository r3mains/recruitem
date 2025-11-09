using backend.DTOs;
using backend.DTOs.Profile;

namespace backend.Repositories.IRepositories;

public interface IProfileRepository
{
  Task<EmployeeWithDetailsDto?> GetEmployeeProfileAsync(string userId);
  Task<EmployeeDto> CreateEmployeeProfileAsync(string userId, CreateEmployeeDto createDto);
  Task<EmployeeDto> UpdateEmployeeProfileAsync(string userId, UpdateEmployeeDto updateDto);
  Task DeleteEmployeeProfileAsync(string userId);

  Task<CandidateWithDetailsDto?> GetCandidateProfileAsync(string userId);
  Task<CandidateDto> CreateCandidateProfileAsync(string userId, CreateCandidateDto createDto);
  Task<CandidateDto> UpdateCandidateProfileAsync(string userId, UpdateCandidateDto updateDto);
  Task DeleteCandidateProfileAsync(string userId);

  Task HandleRoleChangeAsync(string userId, IList<string> newRoles, IList<string> previousRoles);
}
