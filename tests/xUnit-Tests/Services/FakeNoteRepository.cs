using DataAccess.Data.NoteRepo;
using DataAccess.Models;

namespace XUnit_Tests.Services;

internal sealed class FakeNoteRepository : INoteRepository
{
    public List<NoteModel> Items { get; } = [];
    public List<NoteModel> Created { get; } = [];
    public List<NoteModel> Updated { get; } = [];
    public List<NoteModel> Deleted { get; } = [];

    public Task CreateAsync(NoteModel note, CancellationToken cancellationToken = default)
    {
        Created.Add(note);
        Items.Add(note);
        return Task.CompletedTask;
    }

    public Task<NoteModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.SingleOrDefault(x => x.UserId == userId && x.ItemId == itemId));

    public Task<NoteModel?> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
    {
        Updated.Add(note);
        return Task.FromResult<NoteModel?>(note);
    }

    public Task DeleteAsync(NoteModel note, CancellationToken cancellationToken = default)
    {
        Deleted.Add(note);
        Items.Remove(note);
        return Task.CompletedTask;
    }

    public Task<List<NoteModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.Where(x => x.UserId == userId).ToList());
}
