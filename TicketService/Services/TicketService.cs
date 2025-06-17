using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using TicketService.Common;
using TicketService.Data;
using TicketService.DTOs;
using TicketService.Entities;

namespace TicketService.Services
{
    public class TicketService : ITicketService
    {
        private readonly TicketsDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public TicketService(TicketsDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OperationResult<string>> ReserveTicketsAsync(int eventId, int ticketTypeId, int quantity)
        {
            var httpClient = _httpClientFactory.CreateClient("EventService");
            var response = await httpClient.GetAsync($"/api/Events/{eventId}/tickettype/{ticketTypeId}");

            if (!response.IsSuccessStatusCode)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Failed to fetch event capacity."
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var eventInfoResult = JsonSerializer.Deserialize<OperationResult<EventTicketInfoDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (eventInfoResult?.Data == null || eventInfoResult.Data.Capacity <= 0)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Capacity information missing or invalid."
                };
            }

            int eventCapacity = eventInfoResult.Data.Capacity;

            var reservedCount = await _context.TicketReservations
                .Where(r => r.EventId == eventId && r.TicketTypeId == ticketTypeId && !r.IsPurchased && r.ExpiresAt > DateTime.UtcNow)
                .SumAsync(r => r.Quantity);

            if (reservedCount + quantity > eventCapacity)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Not enough tickets available."
                };
            }

            var reservation = new TicketReservation
            {
                EventId = eventId,
                TicketTypeId = ticketTypeId,
                Quantity = quantity,
                ReservationDate = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsPurchased = false
            };

            _context.TicketReservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new OperationResult<string>
            {
                Success = true,
                Message = "Tickets reserved successfully. Reservation valid for 10 minutes.",
                Data = reservation.TicketReservationId.ToString()
            };
        }

        public async Task<OperationResult<string>> PurchaseTicketsAsync(int reservationId)
        {
            var reservation = await _context.TicketReservations.FindAsync(reservationId);
            if (reservation == null)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Reservation not found."
                };
            }

            if (reservation.ExpiresAt <= DateTime.UtcNow)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Reservation has expired."
                };
            }

            if (reservation.IsPurchased)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Reservation already marked as purchased."
                };
            }

            // Simulated payment logic
            bool paymentSuccessful = true;

            if (paymentSuccessful)
            {
                var httpClient = _httpClientFactory.CreateClient("EventService");

                var payload = new
                {
                    EventId = reservation.EventId,
                    TicketTypeId = reservation.TicketTypeId,
                    Quantity = reservation.Quantity
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/api/Events/reduce-capacity", content);

                if (!response.IsSuccessStatusCode)
                {
                    return new OperationResult<string>
                    {
                        Success = false,
                        Message = "Failed to reduce ticket capacity in Event Service."
                    };
                }

                reservation.IsPurchased = true;
                await _context.SaveChangesAsync();

                return new OperationResult<string>
                {
                    Success = true,
                    Message = "Tickets purchased successfully.",
                    Data = reservation.TicketReservationId.ToString()
                };
            }

            return new OperationResult<string>
            {
                Success = false,
                Message = "Payment failed."
            };
        }

        public async Task<OperationResult<string>> CancelReservationAsync(int reservationId)
        {
            var reservation = await _context.TicketReservations.FindAsync(reservationId);
            if (reservation == null)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Reservation not found."
                };
            }

            if (reservation.IsPurchased)
            {
                return new OperationResult<string>
                {
                    Success = false,
                    Message = "Cannot cancel. Tickets already purchased."
                };
            }

            _context.TicketReservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return new OperationResult<string>
            {
                Success = true,
                Message = "Reservation cancelled successfully."
            };
        }

        public async Task<OperationResult<TicketAvailabilityDto>> GetTicketAvailabilityAsync(int eventId, int ticketTypeId)
        {
            var reservedCount = await _context.TicketReservations
                .Where(r => r.EventId == eventId && r.TicketTypeId == ticketTypeId && !r.IsPurchased && r.ExpiresAt > DateTime.UtcNow)
                .SumAsync(r => r.Quantity);

            var purchasedCount = await _context.TicketReservations
                .Where(r => r.EventId == eventId && r.TicketTypeId == ticketTypeId && r.IsPurchased)
                .SumAsync(r => r.Quantity);

            var httpClient = _httpClientFactory.CreateClient("EventService");
            var response = await httpClient.GetAsync($"/api/Events/{eventId}/tickettype/{ticketTypeId}");

            if (!response.IsSuccessStatusCode)
            {
                return new OperationResult<TicketAvailabilityDto>
                {
                    Success = false,
                    Message = "Failed to retrieve ticket info from EventService."
                };
            }

            var result = await response.Content.ReadFromJsonAsync<OperationResult<EventTicketInfoDto>>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var ticketInfo = result?.Data;

            if (ticketInfo == null)
            {
                return new OperationResult<TicketAvailabilityDto>
                {
                    Success = false,
                    Message = "Ticket info not found."
                };
            }

            int eventCapacity = ticketInfo.Capacity;

            var dto = new TicketAvailabilityDto
            {
                RemainingAfterPurchased = eventCapacity - purchasedCount,
                RemainingAfterPurchasedAndReserved = Math.Max(eventCapacity - purchasedCount - reservedCount, 0)
            };

            return new OperationResult<TicketAvailabilityDto>
            {
                Success = true,
                Message = "Ticket availability retrieved.",
                Data = dto
            };
        }
    }
}
