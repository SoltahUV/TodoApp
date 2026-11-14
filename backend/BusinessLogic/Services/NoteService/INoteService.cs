using BusinessLogic.Dto;
using Shared.Constraints;

namespace BusinessLogic.Services.NoteService
{
    public interface INoteService
    {
        Task<NoteDto> CreateAsync(Guid userId, CreateNoteDto dto, CancellationToken cancellationToken = default);
        Task<NoteDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task PatchByIdAsync(Guid userId, Guid itemId, PatchNoteDto dto, CancellationToken cancellationToken = default);
        Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateNoteDto dto, CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<List<NoteDto>> GetAllAsync(
            Guid userId,
            int skip = 0,
            int take = FieldLimits.DefaultPageSize,
            CancellationToken cancellationToken = default);
    }
}
