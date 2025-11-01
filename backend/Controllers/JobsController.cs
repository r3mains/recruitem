using Backend.Dtos.Jobs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController(IJobService service) : BaseController
{
  [HttpGet("public")]
  [AllowAnonymous]
  public async Task<IActionResult> GetPublicJobs([FromQuery] Guid? statusId, [FromQuery] Guid? jobTypeId)
  {
    var data = await service.GetAll(null, statusId, null);
    if (jobTypeId.HasValue)
      data = data.Where(j => j.JobTypeId == jobTypeId).ToList();
    return Ok(data);
  }

  [HttpGet("{id}/public")]
  [AllowAnonymous]
  public async Task<IActionResult> GetPublicJobById(Guid id)
  {
    var job = await service.GetById(id);
    return NotFoundIfNull(job);
  }

  [HttpGet]
  [Authorize]
  public async Task<IActionResult> GetAll([FromQuery] Guid? recruiterId, [FromQuery] Guid? statusId, [FromQuery] Guid? positionId)
  {
    var data = await service.GetAll(recruiterId, statusId, positionId);
    return Ok(data);
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<IActionResult> GetById(Guid id)
  {
    var job = await service.GetById(id);
    return NotFoundIfNull(job);
  }

  [HttpPost]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<IActionResult> Create(JobCreateDto dto)
  {
    var job = await service.Create(dto);
    return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "RecruiterOrHR")]
  public async Task<IActionResult> Update(Guid id, JobUpdateDto dto)
  {
    var job = await service.Update(id, dto);
    return NotFoundIfNull(job);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> Delete(Guid id)
  {
    await service.Delete(id);
    return NoContent();
  }
}
