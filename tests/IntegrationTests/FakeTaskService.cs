using BusinessLogic.Dto;
using BusinessLogic.Services.TaskService;
using DataAccess.Models;
using Shared.Exceptions;

namespace IntegrationTests;

public class FakeTaskService : ITaskService
{
    private static readonly List<TaskDto> Items = [];

    public static void Clear() => Items.Clear();

    public Task<TaskDto> CreateAsync(Guid userId, CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var item = new TaskDto(
            Guid.NewGuid(),
            dto.Name,
            dto.Description,
            TaskProgressStatus.NotStarted,
            DateTime.UtcNow,
            DateTime.UtcNow);
        Items.Add(item);
        return Task.FromResult(item);
    }

    public Task<TaskDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Task not found");
        return Task.FromResult(item);
    }

    public Task PatchByIdAsync(Guid userId, Guid itemId, PatchTaskDto dto, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Task not found");
        return Task.CompletedTask;
    }

    public Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Task not found");
        return Task.CompletedTask;
    }

    public Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Task not found");
        Items.Remove(item);
        return Task.CompletedTask;
    }

    public Task<List<TaskDto>> GetAllAsync(
        Guid userId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Items.Skip(skip).Take(take).ToList());
    }
}
