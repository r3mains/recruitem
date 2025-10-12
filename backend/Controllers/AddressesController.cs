using Microsoft.AspNetCore.Mvc;
using Backend.Dtos.Addresses;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
  private readonly AppDbContext _context;

  public AddressesController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
  {
    var addresses = await _context.Addresses
        .Include(a => a.City)
            .ThenInclude(c => c!.State)
                .ThenInclude(s => s!.Country)
        .Select(a => new AddressDto
        {
          Id = a.Id,
          AddressLine1 = a.AddressLine1,
          AddressLine2 = a.AddressLine2,
          Locality = a.Locality,
          Pincode = a.Pincode,
          CityId = a.CityId,
          CityName = a.City != null ? a.City.Name : null,
          StateName = a.City != null && a.City.State != null ? a.City.State.Name : null,
          CountryName = a.City != null && a.City.State != null && a.City.State.Country != null ? a.City.State.Country.Name : null
        })
        .ToListAsync();

    return Ok(addresses);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AddressDto>> GetById(Guid id)
  {
    var address = await _context.Addresses
        .Include(a => a.City)
            .ThenInclude(c => c!.State)
                .ThenInclude(s => s!.Country)
        .Where(a => a.Id == id)
        .Select(a => new AddressDto
        {
          Id = a.Id,
          AddressLine1 = a.AddressLine1,
          AddressLine2 = a.AddressLine2,
          Locality = a.Locality,
          Pincode = a.Pincode,
          CityId = a.CityId,
          CityName = a.City != null ? a.City.Name : null,
          StateName = a.City != null && a.City.State != null ? a.City.State.Name : null,
          CountryName = a.City != null && a.City.State != null && a.City.State.Country != null ? a.City.State.Country.Name : null
        })
        .FirstOrDefaultAsync();

    if (address == null)
      return NotFound();

    return Ok(address);
  }

  [HttpPost]
  public async Task<ActionResult<AddressDto>> Create(AddressCreateDto dto)
  {
    var address = new Address
    {
      Id = Guid.NewGuid(),
      AddressLine1 = dto.AddressLine1,
      AddressLine2 = dto.AddressLine2,
      Locality = dto.Locality,
      Pincode = dto.Pincode,
      CityId = dto.CityId
    };

    _context.Addresses.Add(address);
    await _context.SaveChangesAsync();

    var result = new AddressDto
    {
      Id = address.Id,
      AddressLine1 = address.AddressLine1,
      AddressLine2 = address.AddressLine2,
      Locality = address.Locality,
      Pincode = address.Pincode,
      CityId = address.CityId
    };

    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
  }
}
