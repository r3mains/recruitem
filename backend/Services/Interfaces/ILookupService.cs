using Backend.Dtos;

namespace Backend.Services.Interfaces;

public interface ILookupService
{
  Task<List<SkillDto>> GetAllSkills();
  Task<List<JobTypeDto>> GetAllJobTypes();
  Task<List<StatusTypeDto>> GetAllStatusTypes(string? context = null);
  Task<List<QualificationDto>> GetAllQualifications();
  Task<List<CountryDto>> GetAllCountries();
  Task<List<StateDto>> GetStatesByCountry(Guid countryId);
  Task<List<CityDto>> GetCitiesByState(Guid stateId);
}
