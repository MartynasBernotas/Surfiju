using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Organizer")]
    public class TripsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileService _fileService;

        public TripsController(IMediator mediator, IFileService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTrips([FromQuery] bool includeInactive = false)
        {
            var query = new GetAllTripsQuery(includeInactive);
            var trips = await _mediator.Send(query);
            return Ok(trips);
        }

        [HttpGet("organizer")]
        public async Task<IActionResult> GetOrganizerTrips()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) )
            {
                return Unauthorized();
            }

            var query = new GetAllTripsQuery(true, userId);
            var trips = await _mediator.Send(query);
            return Ok(trips);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTripById(Guid id)
        {
            var query = new GetTripByIdQuery(id);
            var trip = await _mediator.Send(query);

            if (trip == null)
            {
                return NotFound();
            }

            return Ok(trip);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto tripDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var command = new CreateTripCommand(tripDto, userId);
            var tripId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetTripById), new { id = tripId }, tripId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(Guid id, [FromBody] UpdateTripDto tripDto)
        {
            if (id != tripDto.Id)
            {
                return BadRequest("Trip ID mismatch");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");
            var command = new UpdateTripCommand(tripDto, userId);
            var result = await _mediator.Send(command);

            if (!result && isAdmin)
            {
                // If the user is an admin but doesn't own the trip, we force the update
                var trip = await _mediator.Send(new GetTripByIdQuery(id));
                if (trip != null)
                {
                    command = new UpdateTripCommand(tripDto, trip.OrganizerId);
                    result = await _mediator.Send(command);
                }
            }

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var command = new DeleteTripCommand(id, userId);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
