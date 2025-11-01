using Backend.Dtos;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class LookupService : ILookupService
{
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public LookupService(AppDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<SkillDto>> GetAllSkills()
  {
    var skills = await _context.Skills.ToListAsync();
    return _mapper.Map<List<SkillDto>>(skills);
  }

  public async Task<List<JobTypeDto>> GetAllJobTypes()
  {
    var jobTypes = await _context.JobTypes.ToListAsync();
    return _mapper.Map<List<JobTypeDto>>(jobTypes);
  }

  public async Task<List<StatusTypeDto>> GetAllStatusTypes(string? context = null)
  {
    var query = _context.StatusTypes.AsQueryable();

    if (!string.IsNullOrEmpty(context))
    {
      query = query.Where(st => st.Context == context);
    }

    var statusTypes = await query.ToListAsync();
    return _mapper.Map<List<StatusTypeDto>>(statusTypes);
  }

  public async Task<List<QualificationDto>> GetAllQualifications()
  {
    var qualifications = await _context.Qualifications.ToListAsync();
    return _mapper.Map<List<QualificationDto>>(qualifications);
  }

  public async Task<List<CountryDto>> GetAllCountries()
  {
    var countries = await _context.Countries.ToListAsync();
    return _mapper.Map<List<CountryDto>>(countries);
  }

  public async Task<List<StateDto>> GetStatesByCountry(Guid countryId)
  {
    var states = await _context.States
        .Include(s => s.Country)
        .Where(s => s.CountryId == countryId)
        .ToListAsync();
    return _mapper.Map<List<StateDto>>(states);
  }

  public async Task<List<CityDto>> GetCitiesByState(Guid stateId)
  {
    var cities = await _context.Cities
        .Include(c => c.State)
        .Where(c => c.StateId == stateId)
        .ToListAsync();
    return _mapper.Map<List<CityDto>>(cities);
  }
}
