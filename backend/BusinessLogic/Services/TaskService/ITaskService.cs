using BusinessLogic.Dto;

namespace BusinessLogic.Services.TaskService
{
    public interface ITaskService
    {
        Task CreateAsync(Guid userId, CreateTaskDto dto, CancellationToken cancellationToken = default);
        Task<TaskDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task PatchByIdAsync(Guid userId, Guid itemId, PatchTaskDto dto, CancellationToken cancellationToken = default);
        Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateTaskDto dto, CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<List<TaskDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
