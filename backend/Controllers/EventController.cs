using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Repositories.IRepositories;
using backend.DTOs.Event;

namespace backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("events")]
[Authorize]
public class EventController(IEventRepository eventRepository, IEmployeeRepository employeeRepository) : ControllerBase
{
  private readonly IEventRepository _eventRepository = eventRepository;
  private readonly IEmployeeRepository _employeeRepository = employeeRepository;

  [HttpGet]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> GetAllEvents()
  {
    try
    {
      var events = await _eventRepository.GetAllEventsAsync();
      return Ok(events);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving events.", error = ex.Message });
    }
  }

  [HttpGet("{id}")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> GetEvent(Guid id)
  {
    try
    {
      var eventDto = await _eventRepository.GetEventByIdAsync(id);
      if (eventDto == null)
        return NotFound(new { message = "The event could not be found" });

      return Ok(eventDto);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving the event.", error = ex.Message });
    }
  }

  [HttpPost]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
  {
    try
    {
      // Resolve User ID to Employee ID
      var employee = await _employeeRepository.GetEmployeeByUserIdAsync(dto.CreatedBy);
      if (employee == null)
      {
        return BadRequest(new { message = "Only employees can create events. The current user is not registered as an employee." });
      }

      // Create new DTO with Employee ID
      var eventDtoWithEmployeeId = new CreateEventDto(
        dto.Name,
        dto.Type,
        dto.Location,
        dto.Date,
        employee.Id
      );

      var eventDto = await _eventRepository.CreateEventAsync(eventDtoWithEmployeeId);
      return CreatedAtAction(nameof(GetEvent), new { id = eventDto.Id }, eventDto);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while creating the event.", error = ex.Message });
    }
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDto dto)
  {
    try
    {
      var eventDto = await _eventRepository.UpdateEventAsync(id, dto);
      if (eventDto == null)
        return NotFound(new { message = "The event you're trying to update could not be found" });

      return Ok(eventDto);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating the event.", error = ex.Message });
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> DeleteEvent(Guid id)
  {
    try
    {
      var result = await _eventRepository.DeleteEventAsync(id);
      if (!result)
        return NotFound(new { message = "The event you're trying to delete could not be found" });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while deleting the event.", error = ex.Message });
    }
  }

  [HttpGet("{id}/candidates")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> GetEventCandidates(Guid id)
  {
    try
    {
      var candidates = await _eventRepository.GetEventCandidatesAsync(id);
      return Ok(candidates);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while retrieving event candidates.", error = ex.Message });
    }
  }

  [HttpPost("register-candidate")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateToEventDto dto)
  {
    try
    {
      var eventCandidate = await _eventRepository.RegisterCandidateAsync(dto);
      return Ok(eventCandidate);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while registering the candidate.", error = ex.Message });
    }
  }

  [HttpPut("event-candidates/{id}/status")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> UpdateCandidateStatus(Guid id, [FromBody] UpdateEventCandidateStatusDto dto)
  {
    try
    {
      var eventCandidate = await _eventRepository.UpdateCandidateStatusAsync(id, dto);
      if (eventCandidate == null)
        return NotFound(new { message = "The event candidate registration could not be found" });

      return Ok(eventCandidate);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while updating candidate status.", error = ex.Message });
    }
  }

  [HttpDelete("event-candidates/{id}")]
  [Authorize(Policy = "ManageEvents")]
  public async Task<IActionResult> RemoveCandidateFromEvent(Guid id)
  {
    try
    {
      var result = await _eventRepository.RemoveCandidateFromEventAsync(id);
      if (!result)
        return NotFound(new { message = "The event candidate registration could not be found" });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "An error occurred while removing the candidate from event.", error = ex.Message });
    }
  }
}
