namespace TicketService.DTOs
{
    public class EventTicketInfoDto
    {
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public string EventName { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
    public class TicketInfoResponseDto
    {
        public EventTicketInfoDto AvailableTickets { get; set; }
    }
}
