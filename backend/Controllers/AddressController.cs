using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using backend.Repositories.IRepositories;
using backend.DTOs.Location;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class AddressController(
  IAddressRepository addressRepository,
  IMapper mapper) : ControllerBase
{
  private readonly IAddressRepository _addressRepository = addressRepository;
  private readonly IMapper _mapper = mapper;

  // GET /api/v1/addresses
  [HttpGet]
  public async Task<IActionResult> GetAllAddresses()
  {
    var addresses = await _addressRepository.GetAllAddressesAsync();
    var addressDtos = _mapper.Map<IEnumerable<AddressDto>>(addresses);
    return Ok(addressDtos);
  }

  // GET /api/v1/addresses/{id}
  [HttpGet("{id}")]
  public async Task<IActionResult> GetAddressById(Guid id)
  {
    var address = await _addressRepository.GetAddressByIdAsync(id);
    if (address == null)
    {
      return NotFound(new { message = "Address not found" });
    }

    var addressDto = _mapper.Map<AddressDto>(address);
    return Ok(addressDto);
  }
}
