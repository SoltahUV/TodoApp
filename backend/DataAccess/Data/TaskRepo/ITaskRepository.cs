using DataAccess.Models;
using Shared.Constraints;

namespace DataAccess.Data.TaskRepo
{
    public interface ITaskRepository
    {
        Task CreateAsync(TaskModel note, CancellationToken cancellationToken = default);
        Task<TaskModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<TaskModel?> UpdateAsync(TaskModel note, CancellationToken cancellationToken = default);
        Task DeleteAsync(TaskModel note, CancellationToken cancellationToken = default);
        Task<List<TaskModel>> GetAllAsync(
            Guid userId,
            int skip = 0,
            int take = FieldLimits.DefaultPageSize,
            CancellationToken cancellationToken = default);
    }
}
