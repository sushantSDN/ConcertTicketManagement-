using System.ComponentModel.DataAnnotations;

namespace VenueService.Entities
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must not be less than or equal to zero.")]
        public int Capacity { get; set; }
    }
}
