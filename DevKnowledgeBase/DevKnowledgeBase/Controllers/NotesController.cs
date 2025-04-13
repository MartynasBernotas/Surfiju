using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteCommand command)
        {
            var note = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNoteById), new { id = note }, note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteCommand command)
        {
            if (id != command.Id) return BadRequest();
            var success = await _mediator.Send(command);
            return success is not null ? Ok() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(Guid id)
        {
            var note = await _mediator.Send(new GetNoteByIdQuery(id));
            if (note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var queryCommand = new GetAllNotesQuery(page, pageSize, search);
            var notes = await _mediator.Send(queryCommand);
            return Ok(notes);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeleteNoteCommand(id));
            return success ? NoContent() : NotFound();
        }
    }
}
