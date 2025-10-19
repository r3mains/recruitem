using Backend.Dtos.JobApplications;
using Backend.Models;
using Backend.Repositories.Interfaces;
using AutoMapper;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class JobApplicationService(
    IJobApplicationRepository jobApplicationRepository,
    IMapper mapper,
    AppDbContext context)
{
  public async Task<List<JobApplicationDto>> GetAllAsync(Guid? jobId = null, Guid? candidateId = null, Guid? statusId = null)
  {
    var jobApplications = await jobApplicationRepository.GetAll(jobId, candidateId, statusId);
    return mapper.Map<List<JobApplicationDto>>(jobApplications);
  }

  public async Task<JobApplicationDto?> GetByIdAsync(Guid id)
  {
    var jobApplication = await jobApplicationRepository.GetById(id);
    return jobApplication == null ? null : mapper.Map<JobApplicationDto>(jobApplication);
  }

  public async Task<JobApplicationDto> CreateAsync(JobApplicationCreateDto createDto)
  {
    var jobApplication = mapper.Map<JobApplication>(createDto);

    if (jobApplication.StatusId == Guid.Empty)
    {
      var defaultStatus = await GetOrCreateDefaultStatusAsync();
      jobApplication.StatusId = defaultStatus.Id;
    }

    var created = await jobApplicationRepository.Add(jobApplication);
    return mapper.Map<JobApplicationDto>(created);
  }

  private async Task<StatusType> GetOrCreateDefaultStatusAsync()
  {
    var existingStatus = await context.StatusTypes
        .FirstOrDefaultAsync(s => s.Status == "Pending" || s.Status == "Applied" || s.Status == "Submitted");

    if (existingStatus != null)
      return existingStatus;

    var defaultStatus = new StatusType
    {
      Id = Guid.NewGuid(),
      Context = "JobApplication",
      Status = "Pending"
    };

    context.StatusTypes.Add(defaultStatus);
    await context.SaveChangesAsync();
    return defaultStatus;
  }

  public async Task<JobApplicationDto?> UpdateAsync(Guid id, JobApplicationUpdateDto updateDto, Guid reviewerId)
  {
    var jobApplication = mapper.Map<JobApplication>(updateDto);
    jobApplication.ReviewedBy = reviewerId;

    var updated = await jobApplicationRepository.Update(id, jobApplication);
    return updated == null ? null : mapper.Map<JobApplicationDto>(updated);
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    return await jobApplicationRepository.Delete(id);
  }

  public async Task<bool> CanApplyAsync(Guid jobId, Guid candidateId)
  {
    return !(await jobApplicationRepository.ExistsForJobAndCandidate(jobId, candidateId));
  }
}
