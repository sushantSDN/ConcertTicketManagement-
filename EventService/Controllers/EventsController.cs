using EventService.DTOs;
using EventService.Services;

using Microsoft.AspNetCore.Mvc;

namespace EventService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var result = await _eventService.GetAllEventsAsync();
            if (!result.Success) return BadRequest(new { result.Message });
            return Ok(new { result.Message, result.Data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);
            if (!result.Success) return NotFound(new { result.Message });
            return Ok(new { result.Message, result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateEventAsync([FromBody] EventUpsertDto eventDto)
        {
            var result = await _eventService.AddUpdateEventAsync(eventDto);
            if (!result.Success) return BadRequest(new { result.Message });
            return Ok(new { result.Message, result.Data });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result.Success) return NotFound(new { result.Message });
            return Ok(new { result.Message });
        }

        /// <summary>        
        /// This API returns the count of available tickets based on the provided event and ticket type
        /// </summary>
        /// <returns></returns>
        [HttpGet("{eventId}/tickettype/{ticketTypeId}")]
        public async Task<IActionResult> GetEventTicketInfo(int eventId, int ticketTypeId)
        {
            var result = await _eventService.GetEventTicketInfoAsync(eventId, ticketTypeId);
            if (!result.Success) return NotFound(new { result.Message });
            return Ok(new { result.Message, result.Data });
        }

        /// <summary>        
        /// This API is not used here; the ticket quantity will be reduced by the Ticket Service when a ticket is purchased
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("reduce-capacity")]
        public async Task<IActionResult> ReduceCapacity([FromBody] TicketReductionDto dto)
        {
            var result = await _eventService.ReduceCapacityAsync(dto);
            if (!result.Success) return BadRequest(new { result.Message });
            return Ok(new { result.Message });
        }
    }
}