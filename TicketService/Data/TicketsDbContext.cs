using Microsoft.EntityFrameworkCore;

using TicketService.Entities;

namespace TicketService.Data
{
    public class TicketsDbContext : DbContext
    {
        public TicketsDbContext(DbContextOptions<TicketsDbContext> options) : base(options) { }

        public DbSet<TicketReservation> TicketReservations { get; set; }
    }
}
