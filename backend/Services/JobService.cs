using Backend.Dtos.Jobs;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class JobService : IJobService
{
  private readonly IJobRepository _repo;
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public JobService(IJobRepository repo, AppDbContext context, IMapper mapper)
  {
    _repo = repo;
    _context = context;
    _mapper = mapper;
  }

  public async Task<JobDto?> GetById(Guid id)
  {
    var job = await _context.Jobs
        .Include(j => j.JobType)
        .Include(j => j.Position)
        .Include(j => j.Status)
        .Include(j => j.Location)
            .ThenInclude(l => l!.City)
                .ThenInclude(c => c!.State)
        .Include(j => j.Recruiter)
        .FirstOrDefaultAsync(j => j.Id == id);

    return job == null ? null : _mapper.Map<JobDto>(job);
  }

  public async Task<List<JobDto>> GetAll(Guid? recruiterId = null, Guid? statusId = null, Guid? positionId = null)
  {
    var query = _context.Jobs
        .Include(j => j.JobType)
        .Include(j => j.Position)
        .Include(j => j.Status)
        .Include(j => j.Location)
            .ThenInclude(l => l!.City)
                .ThenInclude(c => c!.State)
        .Include(j => j.Recruiter)
        .AsQueryable();

    if (recruiterId.HasValue)
      query = query.Where(j => j.RecruiterId == recruiterId.Value);

    if (statusId.HasValue)
      query = query.Where(j => j.StatusId == statusId.Value);

    if (positionId.HasValue)
      query = query.Where(j => j.PositionId == positionId.Value);

    var jobs = await query.ToListAsync();
    return _mapper.Map<List<JobDto>>(jobs);
  }

  public async Task<JobDto> Create(JobCreateDto dto)
  {
    var job = _mapper.Map<Job>(dto);
    job.Id = Guid.NewGuid();

    await _repo.Add(job);
    return await GetById(job.Id) ?? throw new Exception("Failed to create job");
  }

  public async Task<JobDto?> Update(Guid id, JobUpdateDto dto)
  {
    var job = await _repo.GetById(id);
    if (job == null) return null;

    _mapper.Map(dto, job);
    await _repo.Update(job);

    return await GetById(id);
  }

  public async Task<bool> Delete(Guid id)
  {
    await _repo.DeleteById(id);
    return true;
  }
}
