using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Constraints;

namespace DataAccess.Data.TaskRepo
{
    internal class TaskRepository(AppDbContext context) : ITaskRepository
    {
        public async Task CreateAsync(TaskModel task, CancellationToken cancellationToken = default)
        {
            await context.Tasks.AddAsync(task, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TaskModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            return await context.Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.ItemId == itemId, cancellationToken);
        }

        public async Task<TaskModel?> UpdateAsync(TaskModel task, CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
            return task;
        }
        public async Task DeleteAsync(TaskModel task, CancellationToken cancellationToken = default)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<TaskModel>> GetAllAsync(
            Guid userId,
            int skip = 0,
            int take = FieldLimits.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var safeSkip = Math.Max(0, skip);
            var safeTake = Math.Clamp(take, 1, FieldLimits.MaxPageSize);

            return await context.Tasks
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Created)
                .ThenBy(x => x.ItemId)
                .Skip(safeSkip)
                .Take(safeTake)
                .ToListAsync(cancellationToken);
        }
    }
}
