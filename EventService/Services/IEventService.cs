using EventService.Common;
using EventService.DTOs;
using EventService.Entities;

namespace EventService.Services
{
    public interface IEventService
    {
        Task<OperationResult<List<GetEventDto>>> GetAllEventsAsync();
        Task<OperationResult<Event?>> GetEventByIdAsync(int id);
        Task<OperationResult<Event>> AddUpdateEventAsync(EventUpsertDto eventDto);
        Task<OperationResult<string>> DeleteEventAsync(int id);
        Task<OperationResult<EventTicketInfoDto>> GetEventTicketInfoAsync(int eventId, int ticketTypeId);
        Task<OperationResult<string>> ReduceCapacityAsync(TicketReductionDto dto);
    }
}
