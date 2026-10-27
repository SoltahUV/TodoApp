using BusinessLogic.Dtos;
using DataAccess.Data.TaskRepo;
using DataAccess.Models;

namespace BusinessLogic.Services.TaskService
{
    internal class TaskService(ITaskRepository taskRepository) : ITaskService
    {
        public async Task CreateAsync(Guid userId, CreateTaskDto dto, CancellationToken cancellationToken = default)
        {
            var task = new TaskModel()
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                Status = TaskProgressStatus.NotStarted,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            await taskRepository.CreateAsync(task, cancellationToken);
        }

        public async Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found");

            await taskRepository.DeleteAsync(task, cancellationToken);
        }

        public async Task<TaskModel> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found");

            return task;
        }

        public async Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateTaskDto dto,
            CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found");

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Updated = DateTime.UtcNow;

            await taskRepository.UpdateAsync(task, cancellationToken);
        }

        public async Task PatchByIdAsync(Guid userId, Guid itemId, PatchTaskDto dto, 
            CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);
            
            if (task == null)
                throw new Exception("Task not found");
            
            if (dto.Name != null)
                task.Name = dto.Name;

            if (dto.Description != null)
                task.Description = dto.Description;
            
            if (dto.Status != null)
                task.Status = dto.Status.Value;

            task.Updated = DateTime.UtcNow;

            await taskRepository.UpdateAsync(task, cancellationToken);
        }
        public async Task<List<TaskModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await taskRepository.GetAllAsync(userId, cancellationToken);

        }
    }
}
