namespace EventService.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int VenueId { get; set; }
        public List<TicketType> TicketTypes { get; set; } = new();
    }
}
