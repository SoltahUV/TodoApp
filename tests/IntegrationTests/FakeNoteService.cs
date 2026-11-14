using BusinessLogic.Dto;
using BusinessLogic.Services.NoteService;
using Shared.Exceptions;

namespace IntegrationTests;

public class FakeNoteService : INoteService
{
    private static readonly List<NoteDto> Items = [];

    public static void Clear() => Items.Clear();

    public Task<NoteDto> CreateAsync(Guid userId, CreateNoteDto dto, CancellationToken cancellationToken = default)
    {
        var item = new NoteDto(
            Guid.NewGuid(),
            dto.Name,
            dto.Text,
            DateTime.UtcNow,
            DateTime.UtcNow);
        Items.Add(item);
        return Task.FromResult(item);
    }

    public Task<NoteDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Note not found");
        return Task.FromResult(item);
    }

    public Task PatchByIdAsync(Guid userId, Guid itemId, PatchNoteDto dto, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Note not found");
        return Task.CompletedTask;
    }

    public Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateNoteDto dto, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Note not found");
        return Task.CompletedTask;
    }

    public Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = Items.SingleOrDefault(x => x.ItemId == itemId);
        if (item is null)
            throw new NotFoundException("Note not found");
        Items.Remove(item);
        return Task.CompletedTask;
    }

    public Task<List<NoteDto>> GetAllAsync(
        Guid userId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Items.Skip(skip).Take(take).ToList());
    }
}
