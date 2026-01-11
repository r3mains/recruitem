using backend.Data;
using backend.DTOs.OfferLetter;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class OfferLetterRepository(ApplicationDbContext context) : IOfferLetterRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<OfferLetterDto?> GetByIdAsync(Guid id)
  {
    var offer = await _context.OfferLetters
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Candidate)
          .ThenInclude(c => c.User)
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Job)
      .FirstOrDefaultAsync(o => o.Id == id);

    return offer != null ? MapToDto(offer) : null;
  }

  public async Task<OfferLetterDto?> GetOfferLetterByIdAsync(Guid id)
  {
    return await GetByIdAsync(id);
  }

  public async Task<List<OfferLetterDto>> GetAllAsync()
  {
    var offers = await _context.OfferLetters
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Candidate)
          .ThenInclude(c => c.User)
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Job)
      .OrderByDescending(o => o.CreatedAt)
      .ToListAsync();

    return offers.Select(MapToDto).ToList();
  }

  public async Task<List<OfferLetterDto>> GetByJobApplicationIdAsync(Guid jobApplicationId)
  {
    var offers = await _context.OfferLetters
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Candidate)
          .ThenInclude(c => c.User)
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Job)
      .Where(o => o.JobApplicationId == jobApplicationId)
      .OrderByDescending(o => o.CreatedAt)
      .ToListAsync();

    return offers.Select(MapToDto).ToList();
  }

  public async Task<List<OfferLetterDto>> GetByStatusAsync(string status)
  {
    var offers = await _context.OfferLetters
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Candidate)
          .ThenInclude(c => c.User)
      .Include(o => o.JobApplication)
        .ThenInclude(ja => ja!.Job)
      .Where(o => o.Status == status)
      .OrderByDescending(o => o.CreatedAt)
      .ToListAsync();

    return offers.Select(MapToDto).ToList();
  }

  public async Task<OfferLetterDto> CreateAsync(CreateOfferLetterDto dto, Guid createdBy)
  {
    var offer = new OfferLetter
    {
      Id = Guid.NewGuid(),
      JobApplicationId = dto.JobApplicationId,
      OfferDate = DateTime.UtcNow,
      JoiningDate = dto.JoiningDate,
      Salary = dto.Salary,
      Benefits = dto.Benefits,
      AdditionalTerms = dto.AdditionalTerms,
      ExpiryDate = dto.ExpiryDate ?? DateTime.UtcNow.AddDays(30),
      Status = "Pending",
      CreatedAt = DateTime.UtcNow,
      CreatedBy = createdBy
    };

    _context.OfferLetters.Add(offer);
    await _context.SaveChangesAsync();

    return (await GetByIdAsync(offer.Id))!;
  }

  public async Task<OfferLetterDto?> UpdateAsync(Guid id, UpdateOfferLetterDto dto)
  {
    var offer = await _context.OfferLetters.FindAsync(id);
    if (offer == null) return null;

    if (dto.JoiningDate.HasValue) offer.JoiningDate = dto.JoiningDate.Value;
    if (dto.Salary.HasValue) offer.Salary = dto.Salary.Value;
    if (dto.Benefits != null) offer.Benefits = dto.Benefits;
    if (dto.AdditionalTerms != null) offer.AdditionalTerms = dto.AdditionalTerms;
    if (dto.ExpiryDate.HasValue) offer.ExpiryDate = dto.ExpiryDate.Value;
    
    offer.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return await GetByIdAsync(id);
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var offer = await _context.OfferLetters.FindAsync(id);
    if (offer == null) return false;

    _context.OfferLetters.Remove(offer);
    await _context.SaveChangesAsync();

    return true;
  }

  public async Task<bool> ExistsAsync(Guid id)
  {
    return await _context.OfferLetters.AnyAsync(o => o.Id == id);
  }

  public async Task<bool> AcceptOfferAsync(Guid id)
  {
    var offer = await _context.OfferLetters.FindAsync(id);
    if (offer == null) return false;

    offer.Status = "Accepted";
    offer.AcceptedDate = DateTime.UtcNow;
    offer.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> RejectOfferAsync(Guid id, string reason)
  {
    var offer = await _context.OfferLetters.FindAsync(id);
    if (offer == null) return false;

    offer.Status = "Rejected";
    offer.RejectedDate = DateTime.UtcNow;
    offer.RejectionReason = reason;
    offer.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> UpdatePdfPathAsync(Guid id, string pdfPath)
  {
    var offer = await _context.OfferLetters.FindAsync(id);
    if (offer == null) return false;

    offer.GeneratedPdfPath = pdfPath;
    offer.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return true;
  }

  private static OfferLetterDto MapToDto(OfferLetter offer)
  {
    var candidate = offer.JobApplication?.Candidate;
    var job = offer.JobApplication?.Job;
    var candidateUser = candidate?.User;

    return new OfferLetterDto(
      offer.Id,
      offer.JobApplicationId,
      candidate?.FullName ?? "",
      candidateUser?.Email ?? "",
      job?.Title ?? "",
      "Company Name", // This could come from configuration
      offer.OfferDate,
      offer.JoiningDate,
      offer.Salary,
      offer.Benefits,
      offer.AdditionalTerms,
      offer.Status,
      offer.AcceptedDate,
      offer.RejectedDate,
      offer.RejectionReason,
      offer.ExpiryDate,
      offer.GeneratedPdfPath,
      offer.CreatedAt,
      offer.UpdatedAt,
      offer.CreatedBy
    );
  }
}
