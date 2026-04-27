using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBookings([FromQuery] BookingStatus? status = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetUserBookingsQuery(userId, status);
            var bookings = await _mediator.Send(query);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new CreateBookingCommand(bookingDto.CampId, userId, bookingDto.Participants);
            var result = await _mediator.Send(command);
            return Ok(new { id = result.BookingId, clientSecret = result.ClientSecret });
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> ConfirmBooking(Guid id, [FromBody] ConfirmBookingDto dto)
        {
            await _mediator.Send(new ConfirmBookingCommand(id, dto.PaymentIntentId));
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompleteBooking(Guid id)
        {
            await _mediator.Send(new CompleteBookingCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(Guid id, [FromQuery] string? reason = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new CancelBookingCommand(id, userId, reason);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("/api/organizer/bookings")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> GetOrganizerBookings([FromQuery] Guid? campId = null, [FromQuery] BookingStatus? status = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetOrganizerBookingsQuery(userId, campId, status);
            var bookings = await _mediator.Send(query);
            return Ok(bookings);
        }

        [HttpGet("/api/organizer/bookings/summary")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> GetBookingSummary([FromQuery] Guid? campId = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = new GetCampBookingSummaryQuery(userId, campId);
            var summary = await _mediator.Send(query);
            return Ok(summary);
        }
    }

    public class CreateBookingDto
    {
        public Guid CampId { get; set; }
        public int Participants { get; set; }
    }

    public class ConfirmBookingDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
