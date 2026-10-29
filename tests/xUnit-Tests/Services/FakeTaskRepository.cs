using DataAccess.Data.TaskRepo;
using DataAccess.Models;

namespace XUnit_Tests.Services;

internal sealed class FakeTaskRepository : ITaskRepository
{
    public List<TaskModel> Items { get; } = [];
    public List<TaskModel> Created { get; } = [];
    public List<TaskModel> Updated { get; } = [];
    public List<TaskModel> Deleted { get; } = [];
    public Guid? LastUserId { get; private set; }

    public Task CreateAsync(TaskModel note, CancellationToken cancellationToken = default)
    {
        Created.Add(note);
        Items.Add(note);
        return Task.CompletedTask;
    }

    public Task<TaskModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.SingleOrDefault(x => x.UserId == userId && x.ItemId == itemId));

    public Task<TaskModel?> UpdateAsync(TaskModel note, CancellationToken cancellationToken = default)
    {
        Updated.Add(note);
        return Task.FromResult<TaskModel?>(note);
    }

    public Task DeleteAsync(TaskModel note, CancellationToken cancellationToken = default)
    {
        Deleted.Add(note);
        Items.Remove(note);
        return Task.CompletedTask;
    }

    public Task<List<TaskModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        LastUserId = userId;
        return Task.FromResult(Items.Where(x => x.UserId == userId).ToList());
    }
}
