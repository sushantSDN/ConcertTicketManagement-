using EventService.Common;
using EventService.Data;
using EventService.DTOs;
using EventService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EventService.Services
{
    public class EventService : IEventService
    {
        private readonly EventDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public EventService(EventDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OperationResult<List<GetEventDto>>> GetAllEventsAsync()
        {
            var events = await _context.Events.Include(e => e.TicketTypes).ToListAsync();

            var eventDtos = events.Select(e => new GetEventDto
            {
                EventId = e.EventId,
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                VenueId = e.VenueId,
                TicketTypes = e.TicketTypes.Select(t => new TicketTypeDto
                {
                    TicketTypeId = t.TicketTypeId,
                    TypeName = t.TypeName,
                    Price = t.Price,
                    AvailableQuantity = t.AvailableQuantity
                }).ToList()
            }).ToList();

            return new OperationResult<List<GetEventDto>>
            {
                Success = true,
                Message = "Events fetched successfully.",
                Data = eventDtos
            };
        }

        public async Task<OperationResult<Event?>> GetEventByIdAsync(int id)
        {
            var ev = await _context.Events.Include(e => e.TicketTypes).FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null)
            {
                return new OperationResult<Event?> { Success = false, Message = "Event not found." };
            }

            return new OperationResult<Event?> { Success = true, Message = "Event retrieved successfully.", Data = ev };
        }

        public async Task<OperationResult<Event>> AddUpdateEventAsync(EventUpsertDto eventDto)
        {
            var httpClient = _httpClientFactory.CreateClient("VenueService");

            var response = await httpClient.GetAsync($"/api/Venue/{eventDto.VenueId}");
            if (!response.IsSuccessStatusCode)
            {
                return new OperationResult<Event>
                {
                    Success = false,
                    Message = $"Venue with ID {eventDto.VenueId} not found."
                };
            }

            var venueJson = await response.Content.ReadAsStringAsync();
            var venueWrapper = JsonSerializer.Deserialize<VenueResponseWrapper>(venueJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var venue = venueWrapper?.Data;

            if (venue == null)
            {
                return new OperationResult<Event> { Success = false, Message = "Failed to retrieve venue information." };
            }

            int totalQuantity = eventDto.TicketTypes.Sum(tt => tt.AvailableQuantity);
            if (totalQuantity != venue.Capacity)
            {
                return new OperationResult<Event>
                {
                    Success = false,
                    Message = $"Total ticket quantity ({totalQuantity}) must equal venue capacity ({venue.Capacity})."
                };
            }

            Event ev;

            if (eventDto.EventId == 0)
            {
                ev = new Event();
                _context.Events.Add(ev);
            }
            else
            {
                ev = await _context.Events.Include(e => e.TicketTypes)
                                          .FirstOrDefaultAsync(e => e.EventId == eventDto.EventId);

                if (ev == null)
                {
                    return new OperationResult<Event> { Success = false, Message = $"Event with ID {eventDto.EventId} not found." };
                }

                _context.TicketTypes.RemoveRange(ev.TicketTypes);
            }

            // Map DTO to Entity
            ev.EventName = eventDto.EventName;
            ev.Description = eventDto.Description;
            ev.EventDate = eventDto.EventDate;
            ev.VenueId = eventDto.VenueId;
            ev.TicketTypes = eventDto.TicketTypes.Select(tt => new TicketType
            {
                TicketTypeId = tt.TicketTypeId,
                TypeName = tt.TypeName,
                Price = tt.Price,
                AvailableQuantity = tt.AvailableQuantity
            }).ToList();

            await _context.SaveChangesAsync();

            return new OperationResult<Event>
            {
                Success = true,
                Message = eventDto.EventId == 0 ? "Event created successfully." : "Event updated successfully.",
                Data = ev
            };
        }

        public async Task<OperationResult<string>> DeleteEventAsync(int id)
        {
            var ev = await _context.Events.FindAsync(id);

            if (ev == null)
            {
                return new OperationResult<string> { Success = false, Message = "Event not found." };
            }

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return new OperationResult<string>
            {
                Success = true,
                Message = "Event deleted successfully.",
                Data = "Deleted"
            };
        }

        public async Task<OperationResult<EventTicketInfoDto>> GetEventTicketInfoAsync(int eventId, int ticketTypeId)
        {
            var ticketType = await _context.TicketTypes
                                           .Include(tt => tt.Event)
                                           .FirstOrDefaultAsync(tt => tt.EventId == eventId && tt.TicketTypeId == ticketTypeId);

            if (ticketType == null)
            {
                return new OperationResult<EventTicketInfoDto>
                {
                    Success = false,
                    Message = "Ticket type not found."
                };
            }

            var dto = new EventTicketInfoDto
            {
                EventId = ticketType.EventId,
                TicketTypeId = ticketType.TicketTypeId,
                EventName = ticketType.Event.EventName,
                Price = ticketType.Price,
                TypeName = ticketType.TypeName,
                Capacity = ticketType.AvailableQuantity
            };

            return new OperationResult<EventTicketInfoDto>
            {
                Success = true,
                Message = "Ticket information retrieved successfully.",
                Data = dto
            };
        }

        public async Task<OperationResult<string>> ReduceCapacityAsync(TicketReductionDto dto)
        {
            var ticketType = await _context.TicketTypes
                                           .FirstOrDefaultAsync(t => t.EventId == dto.EventId && t.TicketTypeId == dto.TicketTypeId);

            if (ticketType == null)
            {
                return new OperationResult<string> { Success = false, Message = "Ticket type not found." };
            }

            if (ticketType.AvailableQuantity < dto.Quantity)
            {
                return new OperationResult<string> { Success = false, Message = "Not enough available tickets." };
            }

            ticketType.AvailableQuantity -= dto.Quantity;
            await _context.SaveChangesAsync();

            return new OperationResult<string>
            {
                Success = true,
                Message = "Ticket capacity reduced successfully.",
                Data = "Reduced"
            };
        }
    }
}
