using Microsoft.AspNetCore.Mvc;

using VenueService.DTOs;
using VenueService.Services;

namespace VenueService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpPost("Create-Venue")]
        public async Task<IActionResult> AddVenue([FromBody] AddVenueDto dto)
        {
            var result = await _venueService.AddVenueAsync(dto);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message, result.Data });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVenues()
        {
            var result = await _venueService.GetAllVenueAsync();

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message, result.Data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var result = await _venueService.GetVenueByIdAsync(id);

            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message, result.Data });
        }
    }
}