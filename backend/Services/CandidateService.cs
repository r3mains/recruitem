using Backend.Dtos.Candidates;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class CandidateService : ICandidateService
{
  private readonly ICandidateRepository _repo;
  private readonly AppDbContext _context;

  public CandidateService(ICandidateRepository repo, AppDbContext context)
  {
    _repo = repo;
    _context = context;
  }

  public async Task<CandidateDto?> GetById(Guid id)
  {
    var candidate = await _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
        .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .FirstOrDefaultAsync(c => c.Id == id);

    return candidate == null ? null : MapToCandidateDto(candidate);
  }

  public async Task<CandidateDto?> GetByUserId(Guid userId)
  {
    var candidate = await _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
        .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    return candidate == null ? null : MapToCandidateDto(candidate);
  }

  public async Task<List<CandidateDto>> GetAll()
  {
    var candidates = await _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
        .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
        .Include(c => c.JobApplications)
        .ToListAsync();

    return candidates.Select(MapToCandidateDto).ToList();
  }

  public async Task<CandidateSearchResultDto> Search(string? search, string? skills, int page, int limit)
  {
    var query = _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
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
      Candidates = candidates.Select(MapToCandidateDto).ToList(),
      TotalCount = totalCount,
      Page = page,
      Limit = limit,
      TotalPages = (int)Math.Ceiling((double)totalCount / limit)
    };
  }

  public async Task<CandidateDto> Create(CandidateCreateDto dto)
  {
    var candidate = new Candidate
    {
      Id = Guid.NewGuid(),
      UserId = dto.UserId,
      FullName = dto.FullName,
      ContactNumber = dto.ContactNumber,
      ResumeUrl = dto.ResumeUrl,
      AddressId = dto.AddressId
    };

    await _repo.Add(candidate);

    return await GetById(candidate.Id) ?? throw new Exception("Failed to create candidate");
  }

  public async Task<CandidateDto?> Update(Guid id, CandidateUpdateDto dto)
  {
    var candidate = await _repo.GetById(id);
    if (candidate == null) return null;

    if (!string.IsNullOrWhiteSpace(dto.FullName)) candidate.FullName = dto.FullName;
    if (!string.IsNullOrWhiteSpace(dto.ContactNumber)) candidate.ContactNumber = dto.ContactNumber;
    if (!string.IsNullOrWhiteSpace(dto.ResumeUrl)) candidate.ResumeUrl = dto.ResumeUrl;
    if (dto.AddressId.HasValue) candidate.AddressId = dto.AddressId;

    await _repo.Update(candidate);

    return await GetById(id);
  }

  public async Task<bool> Delete(Guid id)
  {
    await _repo.DeleteById(id);
    return true;
  }

  private static CandidateDto MapToCandidateDto(Candidate candidate)
  {
    var addressDetails = candidate.Address != null
        ? $"{candidate.Address.Street}, {candidate.Address.City?.Name}, {candidate.Address.State} {candidate.Address.ZipCode}"
        : null;

    return new CandidateDto
    {
      Id = candidate.Id,
      UserId = candidate.UserId,
      FullName = candidate.FullName,
      ContactNumber = candidate.ContactNumber,
      ResumeUrl = candidate.ResumeUrl,
      AddressId = candidate.AddressId,
      Email = candidate.User?.Email,
      AddressDetails = addressDetails,
      Skills = candidate.CandidateSkills?.Select(cs => new CandidateSkillDto
      {
        Id = cs.Id,
        CandidateId = cs.CandidateId,
        SkillId = cs.SkillId,
        SkillName = cs.Skill?.Name ?? "",
        YearsOfExperience = cs.YearsOfExperience
      }).ToList(),
      TotalApplications = candidate.JobApplications?.Count ?? 0
    };
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
        address = new Address
        {
          Id = Guid.NewGuid(),
          AddressLine1 = dto.Address.AddressLine1,
          AddressLine2 = dto.Address.AddressLine2,
          Locality = dto.Address.Locality,
          Pincode = dto.Address.Pincode,
          CityId = dto.Address.CityId
        };
        _context.Addresses.Add(address);
      }

      var candidate = new Candidate
      {
        Id = Guid.NewGuid(),
        UserId = user.Id,
        FullName = dto.FullName,
        ContactNumber = dto.ContactNumber,
        ResumeUrl = dto.ResumeUrl,
        AddressId = address?.Id
      };
      _context.Candidates.Add(candidate);

      await _context.SaveChangesAsync();

      foreach (var skillDto in dto.Skills)
      {
        var candidateSkill = new CandidateSkill
        {
          Id = Guid.NewGuid(),
          CandidateId = candidate.Id,
          SkillId = skillDto.SkillId,
          YearsOfExperience = skillDto.YearsOfExperience
        };
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

    candidate.FullName = dto.FullName ?? candidate.FullName;
    candidate.ContactNumber = dto.ContactNumber ?? candidate.ContactNumber;
    candidate.ResumeUrl = dto.ResumeUrl ?? candidate.ResumeUrl;

    if (dto.Address != null)
    {
      if (candidate.Address != null)
      {
        candidate.Address.AddressLine1 = dto.Address.AddressLine1 ?? candidate.Address.AddressLine1;
        candidate.Address.AddressLine2 = dto.Address.AddressLine2 ?? candidate.Address.AddressLine2;
        candidate.Address.Locality = dto.Address.Locality ?? candidate.Address.Locality;
        candidate.Address.Pincode = dto.Address.Pincode ?? candidate.Address.Pincode;
        candidate.Address.CityId = dto.Address.CityId ?? candidate.Address.CityId;
      }
      else
      {
        var newAddress = new Address
        {
          Id = Guid.NewGuid(),
          AddressLine1 = dto.Address.AddressLine1,
          AddressLine2 = dto.Address.AddressLine2,
          Locality = dto.Address.Locality,
          Pincode = dto.Address.Pincode,
          CityId = dto.Address.CityId
        };
        _context.Addresses.Add(newAddress);
        candidate.AddressId = newAddress.Id;
      }
    }

    await _context.SaveChangesAsync();
    return await GetById(id);
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

    return candidates.Select(MapToCandidateDto).ToList();
  }

  public async Task<List<CandidateDto>> GetCandidatesBySkills(List<Guid> skillIds, int? minExperience = null)
  {
    var query = _context.Candidates
        .Include(c => c.User)
        .Include(c => c.Address)
        .Include(c => c.CandidateSkills)
        .ThenInclude(cs => cs.Skill)
        .Where(c => c.CandidateSkills.Any(cs => skillIds.Contains(cs.SkillId)));

    if (minExperience.HasValue)
    {
      query = query.Where(c => c.CandidateSkills.Any(cs => skillIds.Contains(cs.SkillId) && cs.YearsOfExperience >= minExperience));
    }

    var candidates = await query.ToListAsync();
    return candidates.Select(MapToCandidateDto).ToList();
  }

  public async Task<List<CandidateSkillDto>> GetCandidateSkills(Guid candidateId)
  {
    var skills = await _context.CandidateSkills
        .Include(cs => cs.Skill)
        .Where(cs => cs.CandidateId == candidateId)
        .Select(cs => new CandidateSkillDto
        {
          Id = cs.Id,
          CandidateId = cs.CandidateId,
          SkillId = cs.SkillId,
          SkillName = cs.Skill!.Name,
          YearsOfExperience = cs.YearsOfExperience
        })
        .ToListAsync();
    return skills;
  }

  public async Task<CandidateSkillDto> AddCandidateSkill(Guid candidateId, CandidateSkillCreateDto dto)
  {
    var candidateSkill = new CandidateSkill
    {
      Id = Guid.NewGuid(),
      CandidateId = candidateId,
      SkillId = dto.SkillId,
      YearsOfExperience = dto.YearsOfExperience
    };

    _context.CandidateSkills.Add(candidateSkill);
    await _context.SaveChangesAsync();

    var skill = await _context.Skills.FindAsync(dto.SkillId);
    return new CandidateSkillDto
    {
      Id = candidateSkill.Id,
      CandidateId = candidateSkill.CandidateId,
      SkillId = candidateSkill.SkillId,
      SkillName = skill?.Name ?? "Unknown",
      YearsOfExperience = candidateSkill.YearsOfExperience
    };
  }

  public async Task<CandidateSkillDto?> UpdateCandidateSkill(Guid candidateId, Guid skillId, CandidateSkillUpdateDto dto)
  {
    var candidateSkill = await _context.CandidateSkills
        .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId && cs.Id == skillId);

    if (candidateSkill == null) return null;

    candidateSkill.YearsOfExperience = dto.YearsOfExperience;
    await _context.SaveChangesAsync();

    var skill = await _context.Skills.FindAsync(candidateSkill.SkillId);
    return new CandidateSkillDto
    {
      Id = candidateSkill.Id,
      CandidateId = candidateSkill.CandidateId,
      SkillId = candidateSkill.SkillId,
      SkillName = skill?.Name ?? "Unknown",
      YearsOfExperience = candidateSkill.YearsOfExperience
    };
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

  private async Task<Guid> GetCandidateRoleId()
  {
    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Candidate");
    return role?.Id ?? throw new InvalidOperationException("Candidate role not found");
  }
}
