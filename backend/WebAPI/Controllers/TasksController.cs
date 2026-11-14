using BusinessLogic.Dto;
using BusinessLogic.Services.TaskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constraints;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskDto dto, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var taskDto = await taskService.CreateAsync(userId, dto, ct);
            return Created($"/api/tasks/{taskDto.ItemId}", taskDto);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
        {

            var userId = User.GetUserId();
            var result = await taskService.GetByIdAsync(userId, id, ct);
            return Ok(result);
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

            var result = await taskService.GetAllAsync(userId, skip, take, ct);
            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateByIdAsync([FromRoute] Guid id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
        {

            var userId = User.GetUserId();
            await taskService.UpdateByIdAsync(userId, id, dto, ct);
            return NoContent();
        }

        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> PatchByIdAsync( [FromRoute] Guid id, [FromBody] PatchTaskDto dto, CancellationToken ct)
        {

            var userId = User.GetUserId();
            await taskService.PatchByIdAsync(userId, id, dto, ct);
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid id, CancellationToken ct)
        {

            var userId = User.GetUserId();
            await taskService.DeleteByIdAsync(userId, id, ct);
            return NoContent();
        }
    }
}
