using BusinessLogic.Dto;

namespace BusinessLogic.Services.NoteService
{
    public interface INoteService
    {
        Task CreateAsync(Guid userId, CreateNoteDto dto, CancellationToken cancellationToken = default);
        Task<NoteDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task PatchByIdAsync(Guid userId, Guid itemId, PatchNoteDto dto, CancellationToken cancellationToken = default);
        Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateNoteDto dto, CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<List<NoteDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
