using Backend.Dtos.Positions;
using Backend.Models;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class PositionService : IPositionService
{
  private readonly AppDbContext _context;
  private readonly IMapper _mapper;

  public PositionService(AppDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<PositionDto?> GetById(Guid id)
  {
    var position = await _context.Positions
        .Include(p => p.Status)
        .Include(p => p.Reviewer)
        .FirstOrDefaultAsync(p => p.Id == id);

    return position == null ? null : _mapper.Map<PositionDto>(position);
  }

  public async Task<List<PositionDto>> GetAll()
  {
    var positions = await _context.Positions
        .Include(p => p.Status)
        .Include(p => p.Reviewer)
        .ToListAsync();

    return _mapper.Map<List<PositionDto>>(positions);
  }

  public async Task<PositionDto> Create(PositionCreateDto dto)
  {
    var position = _mapper.Map<Position>(dto);
    position.Id = Guid.NewGuid();

    _context.Positions.Add(position);
    await _context.SaveChangesAsync();

    return await GetById(position.Id) ?? throw new Exception("Failed to create position");
  }

  public async Task<PositionDto?> Update(Guid id, PositionUpdateDto dto)
  {
    var position = await _context.Positions.FindAsync(id);
    if (position == null) return null;

    _mapper.Map(dto, position);
    await _context.SaveChangesAsync();

    return await GetById(id);
  }

  public async Task<bool> Delete(Guid id)
  {
    var position = await _context.Positions.FindAsync(id);
    if (position == null) return false;

    _context.Positions.Remove(position);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<PositionDto?> UpdateStatus(Guid id, PositionStatusUpdateDto dto)
  {
    var position = await _context.Positions.FindAsync(id);
    if (position == null) return null;

    position.StatusId = dto.StatusId;
    position.ClosedReason = dto.ClosedReason;
    await _context.SaveChangesAsync();

    return await GetById(id);
  }
}
