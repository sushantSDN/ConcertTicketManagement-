using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

using TicketService.Services;

namespace TicketService.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _reservationService;

        public TicketsController(ITicketService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveTickets(int eventId, int ticketTypeId, int quantity)
        {
            var result = await _reservationService.ReserveTicketsAsync(eventId, ticketTypeId, quantity);

            if (result.Success)
            {
                return Ok(new
                {
                    ReservationId = result.Data,
                    result.Message
                });
            }

            return BadRequest(new { result.Message });
        }

        [HttpPost("purchase/{reservationId}")]
        public async Task<IActionResult> PurchaseTickets(int reservationId)
        {
            var result = await _reservationService.PurchaseTicketsAsync(reservationId);
            if (result.Success)
                return Ok(new { ReservationId = result.Data, result.Message });

            return BadRequest(new { result.Message });
        }

        [HttpPost("cancel/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var result = await _reservationService.CancelReservationAsync(reservationId);
            if (result.Success)
                return Ok(new { result.Message });

            return BadRequest(new { result.Message });
        }

        /// <summary>        
        /// This API returns the count of available tickets based on the provided event and ticket type with reserved and pruhases count 
        /// </summary>
        /// <returns></returns>
        [HttpGet("availability")]
        public async Task<IActionResult> GetTicketAvailabilityAsync(int eventId, int ticketTypeId)
        {
            var result = await _reservationService.GetTicketAvailabilityAsync(eventId, ticketTypeId);

            if (result.Success)
                return Ok(new { AvailableTickets = result.Data });

            return BadRequest(new { result.Message });
        }
    }
}
