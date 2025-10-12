using Backend.Dtos.Jobs;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class JobService(IJobRepository repo) : IJobService
{
  public async Task<JobDto?> GetById(Guid id)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    return ToDto(e);
  }

  public async Task<List<JobDto>> GetAll(Guid? recruiterId = null, Guid? statusId = null, Guid? positionId = null)
  {
    var list = await repo.GetAll();
    if (recruiterId.HasValue) list = list.Where(j => j.RecruiterId == recruiterId.Value).ToList();
    if (statusId.HasValue) list = list.Where(j => j.StatusId == statusId.Value).ToList();
    if (positionId.HasValue) list = list.Where(j => j.PositionId == positionId.Value).ToList();
    return list.Select(ToDto).ToList();
  }

  public async Task<JobDto> Create(JobCreateDto dto)
  {
    var e = new Job
    {
      Id = Guid.NewGuid(),
      RecruiterId = dto.RecruiterId,
      Title = dto.Title,
      Description = dto.Description,
      JobTypeId = dto.JobTypeId,
      LocationId = dto.LocationId,
      SalaryMin = dto.SalaryMin,
      SalaryMax = dto.SalaryMax,
      PositionId = dto.PositionId,
      StatusId = dto.StatusId,
      ClosedReason = dto.ClosedReason,
      CreatedAt = DateTime.UtcNow
    };
    await repo.Add(e);
    return ToDto(e);
  }

  public async Task<JobDto?> Update(Guid id, JobUpdateDto dto)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    if (!string.IsNullOrWhiteSpace(dto.Title)) e.Title = dto.Title;
    if (!string.IsNullOrWhiteSpace(dto.Description)) e.Description = dto.Description;
    if (dto.JobTypeId.HasValue) e.JobTypeId = dto.JobTypeId.Value;
    if (dto.LocationId.HasValue) e.LocationId = dto.LocationId.Value;
    if (dto.SalaryMin.HasValue) e.SalaryMin = dto.SalaryMin;
    if (dto.SalaryMax.HasValue) e.SalaryMax = dto.SalaryMax;
    if (dto.PositionId.HasValue) e.PositionId = dto.PositionId.Value;
    if (dto.StatusId.HasValue) e.StatusId = dto.StatusId.Value;
    if (dto.ClosedReason != null) e.ClosedReason = dto.ClosedReason;
    e.UpdatedAt = DateTime.UtcNow;
    await repo.Update(e);
    return ToDto(e);
  }

  public async Task<bool> Delete(Guid id)
  {
    await repo.DeleteById(id);
    return true;
  }

  private static JobDto ToDto(Job e) => new()
  {
    Id = e.Id,
    RecruiterId = e.RecruiterId,
    Title = e.Title,
    Description = e.Description,
    JobTypeId = e.JobTypeId,
    LocationId = e.LocationId,
    SalaryMin = e.SalaryMin,
    SalaryMax = e.SalaryMax,
    PositionId = e.PositionId,
    StatusId = e.StatusId,
    ClosedReason = e.ClosedReason,
    CreatedAt = e.CreatedAt,
    UpdatedAt = e.UpdatedAt
  };
}
