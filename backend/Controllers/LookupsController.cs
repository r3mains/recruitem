using Backend.Models;
using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/lookups")]
public class LookupsController(AppDbContext db) : ControllerBase
{
  [HttpGet("status-types")]
  public async Task<IActionResult> GetStatusTypes()
  {
    var data = await db.StatusTypes.AsNoTracking().ToListAsync();
    return Ok(data);
  }

  [HttpGet("job-types")]
  public async Task<IActionResult> GetJobTypes()
  {
    var data = await db.JobTypes.AsNoTracking().ToListAsync();
    return Ok(data);
  }
}
