using Backend.Dtos.Candidates;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class CandidateService : ICandidateService
{
  private readonly ICandidateRepository _repo;
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public CandidateService(ICandidateRepository repo, AppDbContext context, IMapper mapper)
  {
    _repo = repo;
    _context = context;
    _mapper = mapper;
  }

  public async Task<CandidateDto?> GetById(Guid id)
  {
    var candidate = await _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
            .ThenInclude(a => a!.City)
                .ThenInclude(c => c!.State)
        .Include(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .FirstOrDefaultAsync(c => c.Id == id);

    return candidate == null ? null : _mapper.Map<CandidateDto>(candidate);
  }

  public async Task<CandidateDto?> GetByUserId(Guid userId)
  {
    var candidate = await _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
            .ThenInclude(a => a!.City)
                .ThenInclude(c => c!.State)
        .Include(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    return candidate == null ? null : _mapper.Map<CandidateDto>(candidate);
  }

  public async Task<List<CandidateDto>> GetAll()
  {
    var candidates = await _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
            .ThenInclude(a => a!.City)
                .ThenInclude(c => c!.State)
        .Include(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .ToListAsync();

    return _mapper.Map<List<CandidateDto>>(candidates);
  }

  public async Task<CandidateSearchResultDto> Search(string? search, string? skills, int page, int limit)
  {
    var query = _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
            .ThenInclude(a => a!.City)
                .ThenInclude(c => c!.State)
        .Include(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
      var searchLower = search.ToLower();
      query = query.Where(c =>
          c.FullName!.ToLower().Contains(searchLower) ||
          c.User!.Email.ToLower().Contains(searchLower) ||
          c.ContactNumber!.Contains(search));
    }

    if (!string.IsNullOrEmpty(skills))
    {
      var skillNames = skills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim().ToLower())
                            .ToList();
      query = query.Where(c => c.CandidateSkills.Any(cs =>
          skillNames.Contains(cs.Skill!.Name.ToLower())));
    }

    var totalCount = await query.CountAsync();
    var candidates = await query
        .Skip((page - 1) * limit)
        .Take(limit)
        .ToListAsync();

    return new CandidateSearchResultDto
    {
      Candidates = _mapper.Map<List<CandidateDto>>(candidates),
      TotalCount = totalCount,
      Page = page,
      Limit = limit,
      TotalPages = (int)Math.Ceiling((double)totalCount / limit)
    };
  }

  public async Task<CandidateDto> Create(CandidateCreateDto dto)
  {
    var candidate = _mapper.Map<Candidate>(dto);
    candidate.Id = Guid.NewGuid();

    await _repo.Add(candidate);

    return await GetById(candidate.Id) ?? throw new Exception("Failed to create candidate");
  }

  public async Task<CandidateDto?> Update(Guid id, CandidateUpdateDto dto)
  {
    var candidate = await _repo.GetById(id);
    if (candidate == null) return null;

    _mapper.Map(dto, candidate);
    await _repo.Update(candidate);

    return await GetById(id);
  }

  public async Task<bool> Delete(Guid id)
  {
    await _repo.DeleteById(id);
    return true;
  }

  public async Task<List<CandidateDto>> SearchCandidates(CandidateSearchDto searchDto)
  {
    var query = _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
        .ThenInclude(a => a!.City)
        .ThenInclude(c => c!.State)
        .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
        .Include(c => c.CandidateQualifications)
        .ThenInclude(cq => cq.Qualification)
        .AsQueryable();

    if (!string.IsNullOrEmpty(searchDto.Name))
    {
      query = query.Where(c => c.FullName!.Contains(searchDto.Name));
    }

    if (!string.IsNullOrEmpty(searchDto.Email))
    {
      query = query.Where(c => c.User!.Email.Contains(searchDto.Email));
    }

    if (searchDto.SkillIds != null && searchDto.SkillIds.Any())
    {
      query = query.Where(c => c.CandidateSkills.Any(cs => searchDto.SkillIds.Contains(cs.SkillId)));
    }

    if (searchDto.MinExperience.HasValue)
    {
      query = query.Where(c => c.CandidateSkills.Any(cs => cs.YearsOfExperience >= searchDto.MinExperience));
    }

    if (searchDto.MaxExperience.HasValue)
    {
      query = query.Where(c => c.CandidateSkills.Any(cs => cs.YearsOfExperience <= searchDto.MaxExperience));
    }

    if (searchDto.CityId.HasValue)
    {
      query = query.Where(c => c.Address!.CityId == searchDto.CityId);
    }

    if (searchDto.StateId.HasValue)
    {
      query = query.Where(c => c.Address!.City!.StateId == searchDto.StateId);
    }

    var candidates = await query
        .Skip((searchDto.Page - 1) * searchDto.PageSize)
        .Take(searchDto.PageSize)
        .ToListAsync();

    return _mapper.Map<List<CandidateDto>>(candidates);
  }

  public async Task<List<CandidateDto>> GetCandidatesBySkills(List<Guid> skillIds, int? minExperience = null)
  {
    var query = _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
            .ThenInclude(a => a!.City)
                .ThenInclude(c => c!.State)
        .Include(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
        .Where(c => c.CandidateSkills.Any(cs => skillIds.Contains(cs.SkillId)));

    if (minExperience.HasValue)
    {
      query = query.Where(c => c.CandidateSkills.Any(cs => skillIds.Contains(cs.SkillId) && cs.YearsOfExperience >= minExperience));
    }

    var candidates = await query.ToListAsync();
    return _mapper.Map<List<CandidateDto>>(candidates);
  }

  public async Task<List<CandidateSkillDto>> GetCandidateSkills(Guid candidateId)
  {
    var skills = await _context.CandidateSkills
        .Include(cs => cs.Skill)
        .Where(cs => cs.CandidateId == candidateId)
        .ToListAsync();

    return _mapper.Map<List<CandidateSkillDto>>(skills);
  }

  public async Task<CandidateSkillDto> AddCandidateSkill(Guid candidateId, CandidateSkillCreateDto dto)
  {
    var candidateSkill = _mapper.Map<CandidateSkill>(dto);
    candidateSkill.Id = Guid.NewGuid();
    candidateSkill.CandidateId = candidateId;

    _context.CandidateSkills.Add(candidateSkill);
    await _context.SaveChangesAsync();

    var skillWithRelations = await _context.CandidateSkills
        .Include(cs => cs.Skill)
        .FirstAsync(cs => cs.Id == candidateSkill.Id);

    return _mapper.Map<CandidateSkillDto>(skillWithRelations);
  }

  public async Task<CandidateSkillDto?> UpdateCandidateSkill(Guid candidateId, Guid skillId, CandidateSkillUpdateDto dto)
  {
    var candidateSkill = await _context.CandidateSkills
        .Include(cs => cs.Skill)
        .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId && cs.Id == skillId);

    if (candidateSkill == null) return null;

    _mapper.Map(dto, candidateSkill);
    await _context.SaveChangesAsync();

    return _mapper.Map<CandidateSkillDto>(candidateSkill);
  }

  public async Task<bool> RemoveCandidateSkill(Guid candidateId, Guid skillId)
  {
    var candidateSkill = await _context.CandidateSkills
        .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId && cs.Id == skillId);

    if (candidateSkill == null) return false;

    _context.CandidateSkills.Remove(candidateSkill);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<CandidateDto> CreateCandidateProfile(CreateCandidateDto dto)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var user = new User
      {
        Id = Guid.NewGuid(),
        Email = dto.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        RoleId = await GetCandidateRoleId(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };
      _context.Users.Add(user);

      Address? address = null;
      if (dto.Address != null)
      {
        address = _mapper.Map<Address>(dto.Address);
        address.Id = Guid.NewGuid();
        _context.Addresses.Add(address);
      }

      var candidate = _mapper.Map<Candidate>(dto);
      candidate.Id = Guid.NewGuid();
      candidate.UserId = user.Id;
      candidate.AddressId = address?.Id;
      _context.Candidates.Add(candidate);

      await _context.SaveChangesAsync();

      foreach (var skillDto in dto.Skills)
      {
        var candidateSkill = _mapper.Map<CandidateSkill>(skillDto);
        candidateSkill.Id = Guid.NewGuid();
        candidateSkill.CandidateId = candidate.Id;
        _context.CandidateSkills.Add(candidateSkill);
      }

      foreach (var qualDto in dto.Qualifications)
      {
        var candidateQual = new CandidateQualification
        {
          Id = Guid.NewGuid(),
          CandidateId = candidate.Id,
          QualificationId = qualDto.QualificationId,
          YearCompleted = qualDto.YearCompleted,
          Grade = qualDto.Grade
        };
        _context.CandidateQualifications.Add(candidateQual);
      }

      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return await GetById(candidate.Id) ?? throw new InvalidOperationException("Failed to retrieve created candidate");
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<CandidateDto?> UpdateCandidateProfile(Guid id, UpdateCandidateDto dto)
  {
    var candidate = await _context.Candidates
        .Include(c => c.Address)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (candidate == null) return null;

    _mapper.Map(dto, candidate);

    if (dto.Address != null)
    {
      if (candidate.Address != null)
      {
        _mapper.Map(dto.Address, candidate.Address);
      }
      else
      {
        var newAddress = _mapper.Map<Address>(dto.Address);
        newAddress.Id = Guid.NewGuid();
        _context.Addresses.Add(newAddress);
        candidate.AddressId = newAddress.Id;
      }
    }

    await _context.SaveChangesAsync();
    return await GetById(id);
  }

  private async Task<Guid> GetCandidateRoleId()
  {
    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Candidate");
    return role?.Id ?? throw new InvalidOperationException("Candidate role not found");
  }
}
