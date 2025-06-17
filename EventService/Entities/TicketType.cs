using System.Text.Json.Serialization;

namespace EventService.Entities
{
    public class TicketType
    {
        public int TicketTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }

        public int EventId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; } = null!;
    }
}
