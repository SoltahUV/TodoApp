using BusinessLogic.Dto;
using BusinessLogic.Services.TaskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;
using WebAPI.Models;

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
            [FromQuery] PaginationQuery pagination,
            CancellationToken ct = default)
        {
            var userId = User.GetUserId();
            var result = await taskService.GetAllAsync(userId, pagination.Skip, pagination.Take, ct);
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
