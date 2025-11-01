using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/lookups")]
public class LookupsController : BaseController
{
  private readonly ILookupService _lookupService;

  public LookupsController(ILookupService lookupService)
  {
    _lookupService = lookupService;
  }

  [HttpGet("status-types")]
  public async Task<IActionResult> GetStatusTypes([FromQuery] string? context = null)
  {
    var data = await _lookupService.GetAllStatusTypes(context);
    return Ok(data);
  }

  [HttpGet("job-types")]
  public async Task<IActionResult> GetJobTypes()
  {
    var data = await _lookupService.GetAllJobTypes();
    return Ok(data);
  }

  [HttpGet("skills")]
  public async Task<IActionResult> GetSkills()
  {
    var data = await _lookupService.GetAllSkills();
    return Ok(data);
  }

  [HttpGet("qualifications")]
  public async Task<IActionResult> GetQualifications()
  {
    var data = await _lookupService.GetAllQualifications();
    return Ok(data);
  }

  [HttpGet("countries")]
  public async Task<IActionResult> GetCountries()
  {
    var data = await _lookupService.GetAllCountries();
    return Ok(data);
  }

  [HttpGet("states/{countryId}")]
  public async Task<IActionResult> GetStatesByCountry(Guid countryId)
  {
    var data = await _lookupService.GetStatesByCountry(countryId);
    return Ok(data);
  }

  [HttpGet("cities/{stateId}")]
  public async Task<IActionResult> GetCitiesByState(Guid stateId)
  {
    var data = await _lookupService.GetCitiesByState(stateId);
    return Ok(data);
  }
}
