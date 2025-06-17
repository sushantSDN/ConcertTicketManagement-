namespace EventService.DTOs
{
    public class TicketReductionDto
    {
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
