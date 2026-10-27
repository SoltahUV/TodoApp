using BusinessLogic.Dtos;
using BusinessLogic.Services.TaskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Task")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskDto dto, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await taskService.CreateAsync(userId, dto, ct);
            return NoContent();
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await taskService.GetByIdAsync(userId, id, ct);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await taskService.GetAllAsync(userId, ct);
            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateByIdAsync([FromRoute] Guid id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await taskService.UpdateByIdAsync(userId, id, dto, ct);
            return NoContent();
        }

        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> PatchByIdAsync( [FromRoute] Guid id, [FromBody] PatchTaskDto dto, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await taskService.PatchByIdAsync(userId, id, dto, ct);
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid id, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await taskService.DeleteByIdAsync(userId, id, ct);
            return NoContent();
        }
    }
}
