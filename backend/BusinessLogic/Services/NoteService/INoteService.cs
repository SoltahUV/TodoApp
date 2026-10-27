using BusinessLogic.Dtos;
using DataAccess.Models;

namespace BusinessLogic.Services.NoteService
{
    public interface INoteService
    {
        Task CreateAsync(Guid userId, CreateNoteDto dto, CancellationToken cancellationToken = default);
        Task<NoteModel> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellation = default);
        Task PatchByIdAsync(Guid userId, Guid itemId, PatchNoteDto dto, CancellationToken cancellationToken = default);
        Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateNoteDto dto, 
            CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<List<NoteModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
