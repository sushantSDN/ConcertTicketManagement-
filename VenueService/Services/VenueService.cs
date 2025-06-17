using Microsoft.EntityFrameworkCore;
using VenueService.Common;
using VenueService.Data;
using VenueService.DTOs;
using VenueService.Entities;

namespace VenueService.Services
{
    public class VenueService : IVenueService
    {
        private readonly VenueDbContext _context;

        public VenueService(VenueDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<OperationResult<Venue>> AddVenueAsync(AddVenueDto dto)
        {
            if (dto == null)
            {
                return new OperationResult<Venue>
                {
                    Success = false,
                    Message = "Invalid venue details provided."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return new OperationResult<Venue>
                {
                    Success = false,
                    Message = "Venue name is required."
                };
            }

            if (dto.Capacity <= 0)
            {
                return new OperationResult<Venue>
                {
                    Success = false,
                    Message = "Capacity must be greater than zero."
                };
            }

            var venue = new Venue
            {
                Name = dto.Name.Trim(),
                Location = dto.Location?.Trim(),
                Capacity = dto.Capacity
            };

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            return new OperationResult<Venue>
            {
                Success = true,
                Message = "Venue created successfully.",
                Data = venue
            };
        }

        public async Task<OperationResult<List<Venue>>> GetAllVenueAsync()
        {
            var venues = await _context.Venues.ToListAsync();

            return new OperationResult<List<Venue>>
            {
                Success = true,
                Message = "Venue list retrieved successfully.",
                Data = venues
            };
        }

        public async Task<OperationResult<Venue>> GetVenueByIdAsync(int id)
        {
            if (id <= 0)
            {
                return new OperationResult<Venue>
                {
                    Success = false,
                    Message = "Invalid venue ID."
                };
            }

            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return new OperationResult<Venue>
                {
                    Success = false,
                    Message = $"Venue with ID {id} not found."
                };
            }

            return new OperationResult<Venue>
            {
                Success = true,
                Message = "Venue retrieved successfully.",
                Data = venue
            };
        }
    }
}
