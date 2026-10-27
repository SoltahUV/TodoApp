using BusinessLogic.Dtos;
using DataAccess.Models;

namespace BusinessLogic.Services.TaskService
{
    public interface ITaskService
    {
        Task CreateAsync(Guid userId, CreateTaskDto dto,
                    CancellationToken cancellationToken = default);
        Task<TaskModel> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellation = default);
        Task PatchByIdAsync(Guid userId, Guid itemId, PatchTaskDto dto,
                    CancellationToken cancellationToken = default);
        Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateTaskDto dto,
                    CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<List<TaskModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
