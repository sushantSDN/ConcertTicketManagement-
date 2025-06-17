using VenueService.Common;
using VenueService.DTOs;
using VenueService.Entities;

namespace VenueService.Services
{
    public interface IVenueService
    {
        Task<OperationResult<Venue>> AddVenueAsync(AddVenueDto dto);
        Task<OperationResult<List<Venue>>> GetAllVenueAsync();
        Task<OperationResult<Venue>> GetVenueByIdAsync(int id);
    }
}
