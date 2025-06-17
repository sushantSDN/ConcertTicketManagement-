namespace TicketService.Entities
{
    public class TicketReservation
    {
        public int TicketReservationId { get; set; }
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsPurchased { get; set; }
    }
}
