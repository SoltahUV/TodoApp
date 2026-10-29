using BusinessLogic.Dto;
using BusinessLogic.Services.TaskService;
using DataAccess.Models;
using Shared.Exceptions;

namespace XUnit_Tests.Services;

public class TaskServiceTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private readonly FakeTaskRepository _repository = new();
    private readonly ITaskService _service;

    public TaskServiceTests() => _service = new TaskService(_repository);

    [Fact]
    public async Task CreateAsync_StoresTaskForCurrentUser()
    {
        await _service.CreateAsync(UserId, new CreateTaskDto("Task", "Description"));

        var created = Assert.Single(_repository.Created);
        Assert.Equal(UserId, created.UserId);
        Assert.Equal("Task", created.Name);
        Assert.Equal("Description", created.Description);
        Assert.Equal(TaskProgressStatus.NotStarted, created.Status);
        Assert.NotEqual(default, created.Created);
        Assert.Equal(created.Created, created.Updated);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDtoWithoutUserData()
    {
        var task = AddTask();

        var dto = await _service.GetByIdAsync(UserId, task.ItemId);

        Assert.IsType<TaskDto>(dto);
        Assert.Equal(task.ItemId, dto.ItemId);
        Assert.Equal(task.Name, dto.Name);
        Assert.Equal(task.Description, dto.Description);
        Assert.Equal(task.Status, dto.Status);
        Assert.Equal(task.Created, dto.Created);
        Assert.Equal(task.Updated, dto.Updated);
        Assert.Null(typeof(TaskDto).GetProperty(nameof(TaskModel.UserId)));
    }

    [Fact]
    public async Task GetByIdAsync_ForeignUser_ThrowsNotFound()
    {
        var task = AddTask();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(Guid.NewGuid(), task.ItemId));
    }

    [Fact]
    public async Task UpdateByIdAsync_ReplacesAllMutableFields()
    {
        var task = AddTask(status: TaskProgressStatus.InProgress);
        var before = task.Updated;

        await _service.UpdateByIdAsync(
            UserId,
            task.ItemId,
            new UpdateTaskDto("Updated", null, TaskProgressStatus.Done));

        Assert.Equal("Updated", task.Name);
        Assert.Null(task.Description);
        Assert.Equal(TaskProgressStatus.Done, task.Status);
        Assert.True(task.Updated > before);
        Assert.Single(_repository.Updated);
    }

    [Fact]
    public async Task PatchByIdAsync_WithNulls_KeepsExistingValues()
    {
        var task = AddTask(status: TaskProgressStatus.InProgress, description: "keep");

        await _service.PatchByIdAsync(UserId, task.ItemId, new PatchTaskDto(null, null, null));

        Assert.Equal("Task", task.Name);
        Assert.Equal("keep", task.Description);
        Assert.Equal(TaskProgressStatus.InProgress, task.Status);
    }

    [Fact]
    public async Task PatchByIdAsync_UpdatesOnlyProvidedFields()
    {
        var task = AddTask(status: TaskProgressStatus.InProgress, description: "keep");

        await _service.PatchByIdAsync(
            UserId,
            task.ItemId,
            new PatchTaskDto("Renamed", null, TaskProgressStatus.Done));

        Assert.Equal("Renamed", task.Name);
        Assert.Equal("keep", task.Description);
        Assert.Equal(TaskProgressStatus.Done, task.Status);
    }

    [Fact]
    public async Task DeleteByIdAsync_RemovesTask()
    {
        var task = AddTask();

        await _service.DeleteByIdAsync(UserId, task.ItemId);

        Assert.Single(_repository.Deleted);
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task DeleteByIdAsync_MissingTask_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteByIdAsync(UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyCurrentUserTasks()
    {
        _repository.Items.Add(NewTask(UserId));
        _repository.Items.Add(NewTask(UserId));
        _repository.Items.Add(NewTask(Guid.NewGuid()));

        var result = await _service.GetAllAsync(UserId);

        Assert.Equal(2, result.Count);
        Assert.All(result, dto => Assert.IsType<TaskDto>(dto));
    }

    [Fact]
    public async Task GetAllAsync_EmptyRepository_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync(UserId);

        Assert.Empty(result);
    }

    private TaskModel AddTask(
        TaskProgressStatus status = TaskProgressStatus.NotStarted,
        string? description = "Description")
    {
        var task = NewTask(UserId, status, description);
        _repository.Items.Add(task);
        return task;
    }

    private static TaskModel NewTask(
        Guid userId,
        TaskProgressStatus status = TaskProgressStatus.NotStarted,
        string? description = "Description") =>
        new()
        {
            UserId = userId,
            ItemId = Guid.NewGuid(),
            Name = "Task",
            Description = description,
            Status = status,
            Created = DateTime.UtcNow.AddMinutes(-5),
            Updated = DateTime.UtcNow.AddMinutes(-5)
        };
}
