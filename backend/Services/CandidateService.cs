using Backend.Dtos.Candidates;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services;

public class CandidateService(ICandidateRepository repo) : ICandidateService
{
  public async Task<CandidateDto?> GetById(Guid id)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    return new CandidateDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, ContactNumber = e.ContactNumber, ResumeUrl = e.ResumeUrl, AddressId = e.AddressId };
  }

  public async Task<List<CandidateDto>> GetAll()
  {
    var list = await repo.GetAll();
    return list.Select(e => new CandidateDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, ContactNumber = e.ContactNumber, ResumeUrl = e.ResumeUrl, AddressId = e.AddressId }).ToList();
  }

  public async Task<CandidateDto> Create(CandidateCreateDto dto)
  {
    var e = new Candidate { Id = Guid.NewGuid(), UserId = dto.UserId, FullName = dto.FullName, ContactNumber = dto.ContactNumber, ResumeUrl = dto.ResumeUrl, AddressId = dto.AddressId };
    await repo.Add(e);
    return new CandidateDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, ContactNumber = e.ContactNumber, ResumeUrl = e.ResumeUrl, AddressId = e.AddressId };
  }

  public async Task<CandidateDto?> Update(Guid id, CandidateUpdateDto dto)
  {
    var e = await repo.GetById(id);
    if (e == null) return null;
    if (!string.IsNullOrWhiteSpace(dto.FullName)) e.FullName = dto.FullName;
    if (!string.IsNullOrWhiteSpace(dto.ContactNumber)) e.ContactNumber = dto.ContactNumber;
    if (!string.IsNullOrWhiteSpace(dto.ResumeUrl)) e.ResumeUrl = dto.ResumeUrl;
    if (dto.AddressId.HasValue) e.AddressId = dto.AddressId;
    await repo.Update(e);
    return new CandidateDto { Id = e.Id, UserId = e.UserId, FullName = e.FullName, ContactNumber = e.ContactNumber, ResumeUrl = e.ResumeUrl, AddressId = e.AddressId };
  }

  public async Task<bool> Delete(Guid id)
  {
    await repo.DeleteById(id);
    return true;
  }
}
