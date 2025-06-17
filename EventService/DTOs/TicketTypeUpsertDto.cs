namespace EventService.DTOs
{
    public class TicketTypeUpsertDto
    {
        public int TicketTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
