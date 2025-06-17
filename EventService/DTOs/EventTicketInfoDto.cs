namespace EventService.DTOs
{
    public class EventTicketInfoDto
    {
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public string EventName { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
