namespace EventService.DTOs
{
    public class GetEventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public int VenueId { get; set; }
        public List<TicketTypeDto> TicketTypes { get; set; } = new();
    }
    public class TicketTypeDto
    {
        public int TicketTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
