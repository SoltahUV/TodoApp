using BusinessLogic.Dto;
using BusinessLogic.Services.NoteService;
using Shared.Constraints;
using WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController(INoteService noteService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateNoteDto dto, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var noteDto = await noteService.CreateAsync(userId, dto, ct);
            return Created($"/api/notes/{noteDto.ItemId}", noteDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int skip = 0,
            [FromQuery] int take = FieldLimits.DefaultPageSize,
            CancellationToken ct = default)
        {

            var userId = User.GetUserId();

            if (skip < 0)
                return BadRequest(new { error = "skip must be non-negative" });
            if (take < 1 || take > FieldLimits.MaxPageSize)
                return BadRequest(new { error = $"take must be between 1 and {FieldLimits.MaxPageSize}" });

            var result = await noteService.GetAllAsync(userId, skip, take, ct);
            return Ok(result);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetNoteAsync([FromRoute] Guid id, CancellationToken ct)
        {

            var userId = User.GetUserId();
            var result = await noteService.GetByIdAsync(userId, id, ct);
            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateByIdAsync([FromRoute] Guid id, [FromBody] UpdateNoteDto dto, CancellationToken ct)
        {

            var userId = User.GetUserId();
            await noteService.UpdateByIdAsync(userId, id, dto, ct);
            return NoContent();
        }

        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> PatchByIdAsync([FromRoute] Guid id, [FromBody] PatchNoteDto dto, CancellationToken ct)
        {

            var userId = User.GetUserId();
            await noteService.PatchByIdAsync(userId, id, dto, ct);
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid id, CancellationToken ct)
        {

            var userId = User.GetUserId();
            await noteService.DeleteByIdAsync(userId, id, ct);
            return NoContent();
        }
    }
}
