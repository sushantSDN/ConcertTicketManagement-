namespace EventService.DTOs
{
    public class VenueDto
    {
        public int VenueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
    public class VenueResponseWrapper
    {
        public VenueDto Data { get; set; }
    }
}
