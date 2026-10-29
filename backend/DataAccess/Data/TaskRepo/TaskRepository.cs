using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Data.TaskRepo
{
    internal class TaskRepository(AppDbContext context, ILogger<TaskRepository> logger) : ITaskRepository
    {
        public async Task CreateAsync(TaskModel task, CancellationToken cancellationToken = default)
        {
            await context.Tasks.AddAsync(task, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created task for user {UserId} and item {ItemId}", task.UserId, task.ItemId);
        }

        public async Task<TaskModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.ItemId == itemId, cancellationToken);
            logger.LogInformation("Retrieved task for user {UserId} and item {ItemId}", userId, itemId);
            return task;
        }

        public async Task<TaskModel?> UpdateAsync(TaskModel task, CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Updated task for user {UserId} and item {ItemId}", task.UserId, task.ItemId);
            return task;
        }
        public async Task DeleteAsync(TaskModel task, CancellationToken cancellationToken = default)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Deleted task for user {UserId} and item {ItemId}", task.UserId, task.ItemId);
        }
        public async Task<List<TaskModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var tasks = await context.Tasks
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            logger.LogInformation("Retrieved all tasks for user {UserId}", userId);
            return tasks;
        }
    }
}
