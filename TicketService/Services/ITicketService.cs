using TicketService.Common;
using TicketService.DTOs;

namespace TicketService.Services
{
    public interface ITicketService
    {
        Task<OperationResult<string>> ReserveTicketsAsync(int eventId, int ticketTypeId, int quantity);
        Task<OperationResult<string>> PurchaseTicketsAsync(int reservationId);
        Task<OperationResult<string>> CancelReservationAsync(int reservationId);
        Task<OperationResult<TicketAvailabilityDto>> GetTicketAvailabilityAsync(int eventId, int ticketTypeId);
    }
}
