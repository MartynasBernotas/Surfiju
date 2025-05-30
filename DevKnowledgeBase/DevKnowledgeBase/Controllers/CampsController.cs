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
    public class CampsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileService _fileService;

        public CampsController(IMediator mediator, IFileService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCamps([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? location = null, [FromQuery] bool includeInactive = false)
        {
            var query = new GetAllCampsQuery(page, pageSize, location, includeInactive);
            var camps = await _mediator.Send(query);
            return Ok(camps);
        }

        [HttpGet("organizer")]
        public async Task<IActionResult> GetOrganizerCamps([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? location = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) )
            {
                return Unauthorized();
            }

            var query = new GetAllCampsQuery(page, pageSize, location, true, userId);
            var camps = await _mediator.Send(query);
            return Ok(camps);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCampById(Guid id)
        {
            var query = new GetCampByIdQuery(id);
            var camp = await _mediator.Send(query);

            if (camp == null)
            {
                return NotFound();
            }

            return Ok(camp);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCamp([FromBody] CreateCampDto CampDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var command = new CreateCampCommand(CampDto, userId);
            var CampId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetCampById), new { id = CampId }, CampId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCamp(Guid id, [FromBody] UpdateCampDto campDto)
        {
            if (id != campDto.Id)
            {
                return BadRequest("Camp ID mismatch");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");
            var command = new UpdateCampCommand(campDto, userId);
            var result = await _mediator.Send(command);

            if (!result && isAdmin)
            {
                // If the user is an admin but doesn't own the Camp, we force the update
                var camp = await _mediator.Send(new GetCampByIdQuery(id));
                if (camp != null)
                {
                    command = new UpdateCampCommand(campDto, camp.OrganizerId);
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
        public async Task<IActionResult> DeleteCamp(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var command = new DeleteCampCommand(id, userId);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
