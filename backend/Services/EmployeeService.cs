using Backend.Dtos.Employees;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class EmployeeService(IEmployeeRepository repo) : IEmployeeService
{
  public async Task<EmployeeDto?> GetById(Guid id)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    return new EmployeeDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, BranchAddressId = e.BranchAddressId, JoiningDate = e.JoiningDate, OfferLetterUrl = e.OfferLetterUrl };
  }

  public async Task<List<EmployeeDto>> GetAll()
  {
    var list = await repo.GetAll();
    return list.Select(e => new EmployeeDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, BranchAddressId = e.BranchAddressId, JoiningDate = e.JoiningDate, OfferLetterUrl = e.OfferLetterUrl }).ToList();
  }

  public async Task<EmployeeDto> Create(EmployeeCreateDto dto)
  {
    var e = new Employee { Id = Guid.NewGuid(), UserId = dto.UserId, FullName = dto.FullName, BranchAddressId = dto.BranchAddressId, JoiningDate = dto.JoiningDate, OfferLetterUrl = dto.OfferLetterUrl };
    await repo.Add(e);
    return new EmployeeDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, BranchAddressId = e.BranchAddressId, JoiningDate = e.JoiningDate, OfferLetterUrl = e.OfferLetterUrl };
  }

  public async Task<EmployeeDto?> Update(Guid id, EmployeeUpdateDto dto)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    if (!string.IsNullOrWhiteSpace(dto.FullName)) e.FullName = dto.FullName;
    if (dto.BranchAddressId.HasValue) e.BranchAddressId = dto.BranchAddressId;
    if (dto.JoiningDate.HasValue) e.JoiningDate = dto.JoiningDate;
    if (!string.IsNullOrWhiteSpace(dto.OfferLetterUrl)) e.OfferLetterUrl = dto.OfferLetterUrl;
    await repo.Update(e);
    return new EmployeeDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, BranchAddressId = e.BranchAddressId, JoiningDate = e.JoiningDate, OfferLetterUrl = e.OfferLetterUrl };
  }

  public async Task<bool> Delete(Guid id)
  {
    await repo.DeleteById(id);
    return true;
  }
}
