using BusinessLogic.Dto;
using BusinessLogic.Mapping;
using DataAccess.Data.TaskRepo;
using DataAccess.Models;
using Shared.Exceptions;

namespace BusinessLogic.Services.TaskService
{
    internal class TaskService(ITaskRepository taskRepository) : ITaskService
    {
        public async Task CreateAsync(Guid userId, CreateTaskDto dto, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var task = new TaskModel()
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                Status = TaskProgressStatus.NotStarted,
                Created = now,
                Updated = now
            };

            await taskRepository.CreateAsync(task, cancellationToken);
        }

        public async Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task not found");

            await taskRepository.DeleteAsync(task, cancellationToken);
        }

        public async Task<TaskDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task not found");

            return task.ToDto();
        }

        public async Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateTaskDto dto,
            CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task not found");

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Status = dto.Status!.Value;
            task.Updated = DateTime.UtcNow;

            await taskRepository.UpdateAsync(task, cancellationToken);
        }

        public async Task PatchByIdAsync(Guid userId, Guid itemId, PatchTaskDto dto,
            CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (dto.Name != null)
                task.Name = dto.Name;

            if (dto.Description != null)
                task.Description = dto.Description;

            if (dto.Status != null)
                task.Status = dto.Status.Value;

            task.Updated = DateTime.UtcNow;

            await taskRepository.UpdateAsync(task, cancellationToken);
        }

        public async Task<List<TaskDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var tasks = await taskRepository.GetAllAsync(userId, cancellationToken);

            return tasks.Select(task => task.ToDto()).ToList();
        }
    }
}
