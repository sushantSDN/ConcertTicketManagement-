using Microsoft.EntityFrameworkCore;

using VenueService.Entities;

namespace VenueService.Data
{
    public class VenueDbContext : DbContext
    {
        public VenueDbContext(DbContextOptions<VenueDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; } = null!;
    }
}
