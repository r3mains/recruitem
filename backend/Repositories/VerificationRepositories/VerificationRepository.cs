using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs.Verification;
using backend.Repositories.IRepositories;

namespace backend.Repositories;

public class VerificationRepository(ApplicationDbContext context) : IVerificationRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<IEnumerable<VerificationListDto>> GetAllVerificationsAsync()
  {
    var verifications = await _context.Verifications
      .Include(v => v.Candidate)
        .ThenInclude(c => c.User)
      .Include(v => v.Document)
        .ThenInclude(d => d.DocumentType)
      .Include(v => v.Status)
      .Include(v => v.VerifiedByEmployee)
        .ThenInclude(e => e.User)
      .ToListAsync();

    return verifications
      .Select(v => new VerificationListDto(
        v.Id,
        v.Candidate?.User?.UserName ?? "Unknown",
        v.Document?.DocumentType?.Type ?? "Unknown",
        v.Status?.Status ?? "Pending",
        v.VerifiedByEmployee?.User?.UserName ?? "Pending",
        v.VerifiedAt
      ))
      .ToList();
  }

  public async Task<IEnumerable<VerificationListDto>> GetVerificationsByCandidateAsync(Guid candidateId)
  {
    var verifications = await _context.Verifications
      .Include(v => v.Candidate)
        .ThenInclude(c => c.User)
      .Include(v => v.Document)
        .ThenInclude(d => d.DocumentType)
      .Include(v => v.Status)
      .Include(v => v.VerifiedByEmployee)
        .ThenInclude(e => e.User)
      .Where(v => v.CandidateId == candidateId)
      .ToListAsync();

    return verifications
      .Select(v => new VerificationListDto(
        v.Id,
        v.Candidate?.User?.UserName ?? "Unknown",
        v.Document?.DocumentType?.Type ?? "Unknown",
        v.Status?.Status ?? "Pending",
        v.VerifiedByEmployee?.User?.UserName ?? "Pending",
        v.VerifiedAt
      ))
      .ToList();
  }

  public async Task<VerificationDto?> GetVerificationByIdAsync(Guid id)
  {
    var v = await _context.Verifications
      .Include(v => v.Candidate)
        .ThenInclude(c => c.User)
      .Include(v => v.Document)
        .ThenInclude(d => d.DocumentType)
      .Include(v => v.Status)
      .Include(v => v.VerifiedByEmployee)
        .ThenInclude(e => e.User)
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    if (v == null) return null;

    return new VerificationDto(
      v.Id,
      v.CandidateId,
      v.Candidate?.User?.UserName ?? "Unknown",
      v.DocumentId,
      v.Document?.Url ?? string.Empty,
      v.Document?.DocumentType?.Type ?? "Unknown",
      v.StatusId,
      v.Status?.Status ?? "Pending",
      v.Comments,
      v.VerifiedBy,
      v.VerifiedByEmployee?.User?.UserName,
      v.VerifiedAt
    );
  }

  public async Task<VerificationDto> CreateVerificationAsync(CreateVerificationDto dto)
  {
    var verification = new Verification
    {
      Id = Guid.NewGuid(),
      CandidateId = dto.CandidateId,
      DocumentId = dto.DocumentId,
      StatusId = dto.StatusId,
      Comments = dto.Comments,
      VerifiedBy = dto.VerifiedBy,
      VerifiedAt = DateTime.UtcNow
    };

    _context.Verifications.Add(verification);
    await _context.SaveChangesAsync();

    return (await GetVerificationByIdAsync(verification.Id))!;
  }

  public async Task<VerificationDto?> UpdateVerificationAsync(Guid id, UpdateVerificationDto dto)
  {
    var verification = await _context.Verifications
      .FirstOrDefaultAsync(v => v.Id == id);

    if (verification == null)
      return null;

    verification.StatusId = dto.StatusId;
    verification.Comments = dto.Comments;
    verification.VerifiedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return await GetVerificationByIdAsync(id);
  }

  public async Task<bool> DeleteVerificationAsync(Guid id)
  {
    var verification = await _context.Verifications
      .FirstOrDefaultAsync(v => v.Id == id);

    if (verification == null)
      return false;

    _context.Verifications.Remove(verification);
    await _context.SaveChangesAsync();
    return true;
  }
}
