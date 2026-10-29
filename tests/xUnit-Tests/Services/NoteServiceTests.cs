using BusinessLogic.Dto;
using BusinessLogic.Services.NoteService;
using DataAccess.Models;
using Shared.Exceptions;

namespace XUnit_Tests.Services;

public class NoteServiceTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private readonly FakeNoteRepository _repository = new();
    private readonly INoteService _service;

    public NoteServiceTests() => _service = new NoteService(_repository);

    [Fact]
    public async Task CreateAsync_StoresNoteForCurrentUser()
    {
        await _service.CreateAsync(UserId, new CreateNoteDto("Note", null));

        var created = Assert.Single(_repository.Created);
        Assert.Equal(UserId, created.UserId);
        Assert.Equal("Note", created.Name);
        Assert.Null(created.Text);
        Assert.NotEqual(default, created.Created);
        Assert.Equal(created.Created, created.Updated);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDtoWithoutUserData()
    {
        var note = AddNote();

        var dto = await _service.GetByIdAsync(UserId, note.ItemId);

        Assert.IsType<NoteDto>(dto);
        Assert.Equal(note.ItemId, dto.ItemId);
        Assert.Equal(note.Name, dto.Name);
        Assert.Equal(note.Text, dto.Text);
        Assert.Null(typeof(NoteDto).GetProperty(nameof(NoteModel.UserId)));
    }

    [Fact]
    public async Task GetByIdAsync_ForeignUser_ThrowsNotFound()
    {
        var note = AddNote();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(Guid.NewGuid(), note.ItemId));
    }

    [Fact]
    public async Task UpdateByIdAsync_ReplacesAllMutableFields()
    {
        var note = AddNote(text: "old");
        var before = note.Updated;

        await _service.UpdateByIdAsync(UserId, note.ItemId, new UpdateNoteDto("Updated", null));

        Assert.Equal("Updated", note.Name);
        Assert.Null(note.Text);
        Assert.True(note.Updated > before);
        Assert.Single(_repository.Updated);
    }

    [Fact]
    public async Task PatchByIdAsync_WithNulls_KeepsExistingValues()
    {
        var note = AddNote(text: "keep");

        await _service.PatchByIdAsync(UserId, note.ItemId, new PatchNoteDto(null, null));

        Assert.Equal("Note", note.Name);
        Assert.Equal("keep", note.Text);
    }

    [Fact]
    public async Task PatchByIdAsync_UpdatesProvidedFields()
    {
        var note = AddNote(text: "old");

        await _service.PatchByIdAsync(UserId, note.ItemId, new PatchNoteDto("Renamed", "new"));

        Assert.Equal("Renamed", note.Name);
        Assert.Equal("new", note.Text);
    }

    [Fact]
    public async Task DeleteByIdAsync_RemovesNote()
    {
        var note = AddNote();

        await _service.DeleteByIdAsync(UserId, note.ItemId);

        Assert.Single(_repository.Deleted);
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyCurrentUserNotes()
    {
        _repository.Items.Add(NewNote(UserId));
        _repository.Items.Add(NewNote(Guid.NewGuid()));

        var result = await _service.GetAllAsync(UserId);

        var dto = Assert.Single(result);
        Assert.IsType<NoteDto>(dto);
    }

    private NoteModel AddNote(string? text = "Text")
    {
        var note = NewNote(UserId, text);
        _repository.Items.Add(note);
        return note;
    }

    private static NoteModel NewNote(Guid userId, string? text = "Text") =>
        new()
        {
            UserId = userId,
            ItemId = Guid.NewGuid(),
            Name = "Note",
            Text = text,
            Created = DateTime.UtcNow.AddMinutes(-5),
            Updated = DateTime.UtcNow.AddMinutes(-5)
        };
}
