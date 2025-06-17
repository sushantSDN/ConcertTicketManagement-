namespace EventService.DTOs
{
    public class EventUpsertDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int VenueId { get; set; }
        public List<TicketTypeUpsertDto> TicketTypes { get; set; } = new();
    }
}
