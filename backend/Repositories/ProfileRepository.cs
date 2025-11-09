using backend.DTOs;
using backend.DTOs.Profile;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using backend.Data;
using backend.Consts;

namespace backend.Repositories;

public class ProfileRepository(
    ApplicationDbContext context,
    UserManager<User> userManager,
    IMapper mapper) : IProfileRepository
{
  private readonly ApplicationDbContext _context = context;
  private readonly UserManager<User> _userManager = userManager;
  private readonly IMapper _mapper = mapper;

  public async Task<EmployeeWithDetailsDto?> GetEmployeeProfileAsync(string userId)
  {
    var employee = await _context.Employees
      .Include(e => e.User)
      .Include(e => e.BranchAddress)
        .ThenInclude(a => a!.City)
          .ThenInclude(c => c!.State)
            .ThenInclude(s => s.Country)
      .FirstOrDefaultAsync(e => e.UserId.ToString() == userId && !e.IsDeleted);

    if (employee == null) return null;

    return _mapper.Map<EmployeeWithDetailsDto>(employee);
  }

  public async Task<EmployeeDto> CreateEmployeeProfileAsync(string userId, CreateEmployeeDto createDto)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      throw new ArgumentException("User not found", nameof(userId));

    var existingEmployee = await _context.Employees
      .FirstOrDefaultAsync(e => e.UserId.ToString() == userId);

    if (existingEmployee != null)
    {
      if (existingEmployee.IsDeleted)
      {
        existingEmployee.IsDeleted = false;
        existingEmployee.UpdatedAt = DateTime.UtcNow;
        existingEmployee.FullName = createDto.FullName;
        existingEmployee.BranchAddressId = createDto.BranchAddressId;
        existingEmployee.JoiningDate = createDto.JoiningDate;
        existingEmployee.OfferLetterUrl = createDto.OfferLetterUrl;

        await _context.SaveChangesAsync();
        return _mapper.Map<EmployeeDto>(existingEmployee);
      }
      else
      {
        throw new InvalidOperationException("Employee profile already exists for this user");
      }
    }

    var employee = new Employee
    {
      Id = Guid.NewGuid(),
      UserId = Guid.Parse(userId),
      FullName = createDto.FullName,
      BranchAddressId = createDto.BranchAddressId,
      JoiningDate = createDto.JoiningDate,
      OfferLetterUrl = createDto.OfferLetterUrl,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      IsDeleted = false
    };

    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();

    return _mapper.Map<EmployeeDto>(employee);
  }

  public async Task<EmployeeDto> UpdateEmployeeProfileAsync(string userId, UpdateEmployeeDto updateDto)
  {
    var employee = await _context.Employees
      .FirstOrDefaultAsync(e => e.UserId.ToString() == userId && !e.IsDeleted);

    if (employee == null)
      throw new ArgumentException("Employee profile not found", nameof(userId));

    employee.FullName = updateDto.FullName;
    employee.BranchAddressId = updateDto.BranchAddressId;
    employee.JoiningDate = updateDto.JoiningDate;
    employee.OfferLetterUrl = updateDto.OfferLetterUrl;
    employee.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return _mapper.Map<EmployeeDto>(employee);
  }

  public async Task DeleteEmployeeProfileAsync(string userId)
  {
    var employee = await _context.Employees
      .FirstOrDefaultAsync(e => e.UserId.ToString() == userId && !e.IsDeleted);

    if (employee != null)
    {
      employee.IsDeleted = true;
      employee.UpdatedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
    }
  }

  public async Task<CandidateWithDetailsDto?> GetCandidateProfileAsync(string userId)
  {
    var candidate = await _context.Candidates
      .Include(c => c.User)
      .Include(c => c.Address)
        .ThenInclude(a => a!.City)
          .ThenInclude(c => c!.State)
            .ThenInclude(s => s.Country)
      .FirstOrDefaultAsync(c => c.UserId.ToString() == userId && !c.IsDeleted);

    if (candidate == null) return null;

    return _mapper.Map<CandidateWithDetailsDto>(candidate);
  }

  public async Task<CandidateDto> CreateCandidateProfileAsync(string userId, CreateCandidateDto createDto)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      throw new ArgumentException("User not found", nameof(userId));

    var existingCandidate = await _context.Candidates
      .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

    if (existingCandidate != null)
    {
      if (existingCandidate.IsDeleted)
      {
        existingCandidate.IsDeleted = false;
        existingCandidate.UpdatedAt = DateTime.UtcNow;
        existingCandidate.FullName = createDto.FullName;
        existingCandidate.ContactNumber = createDto.ContactNumber;
        existingCandidate.AddressId = createDto.AddressId;

        await _context.SaveChangesAsync();
        return _mapper.Map<CandidateDto>(existingCandidate);
      }
      else
      {
        throw new InvalidOperationException("Candidate profile already exists for this user");
      }
    }

    var candidate = new Candidate
    {
      Id = Guid.NewGuid(),
      UserId = Guid.Parse(userId),
      FullName = createDto.FullName,
      ContactNumber = createDto.ContactNumber,
      AddressId = createDto.AddressId,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      IsDeleted = false
    };

    _context.Candidates.Add(candidate);
    await _context.SaveChangesAsync();

    return _mapper.Map<CandidateDto>(candidate);
  }

  public async Task<CandidateDto> UpdateCandidateProfileAsync(string userId, UpdateCandidateDto updateDto)
  {
    var candidate = await _context.Candidates
      .FirstOrDefaultAsync(c => c.UserId.ToString() == userId && !c.IsDeleted);

    if (candidate == null)
      throw new ArgumentException("Candidate profile not found", nameof(userId));

    candidate.FullName = updateDto.FullName;
    candidate.ContactNumber = updateDto.ContactNumber;
    candidate.AddressId = updateDto.AddressId;
    candidate.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return _mapper.Map<CandidateDto>(candidate);
  }

  public async Task DeleteCandidateProfileAsync(string userId)
  {
    var candidate = await _context.Candidates
      .FirstOrDefaultAsync(c => c.UserId.ToString() == userId && !c.IsDeleted);

    if (candidate != null)
    {
      candidate.IsDeleted = true;
      candidate.UpdatedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
    }
  }

  public async Task HandleRoleChangeAsync(string userId, IList<string> newRoles, IList<string> previousRoles)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return;

    var hasEmployeeRole = newRoles.Any(r => r is Roles.Admin or Roles.HR or Roles.Recruiter or Roles.Interviewer or Roles.Reviewer);
    var hadEmployeeRole = previousRoles.Any(r => r is Roles.Admin or Roles.HR or Roles.Recruiter or Roles.Interviewer or Roles.Reviewer);

    if (hasEmployeeRole && !hadEmployeeRole)
    {
      var createEmployeeDto = new CreateEmployeeDto(
        Guid.Parse(userId),
        user.UserName,
        null,
        null,
        null
      );

      try
      {
        await CreateEmployeeProfileAsync(userId, createEmployeeDto);
      }
      catch (InvalidOperationException)
      {
      }
    }
    else if (!hasEmployeeRole && hadEmployeeRole)
    {
      await DeleteEmployeeProfileAsync(userId);
    }

    var hasCandidateRole = newRoles.Contains(Roles.Candidate);
    var hadCandidateRole = previousRoles.Contains(Roles.Candidate);

    if (hasCandidateRole && !hadCandidateRole)
    {
      var createCandidateDto = new CreateCandidateDto(
        Guid.Parse(userId),
        user.UserName,
        null,
        null
      );

      try
      {
        await CreateCandidateProfileAsync(userId, createCandidateDto);
      }
      catch (InvalidOperationException)
      {
      }
    }
    else if (!hasCandidateRole && hadCandidateRole)
    {
      await DeleteCandidateProfileAsync(userId);
    }
  }
}
