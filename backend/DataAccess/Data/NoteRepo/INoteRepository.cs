using DataAccess.Models;
using Shared.Constraints;

namespace DataAccess.Data.NoteRepo
{
    public interface INoteRepository
    {
        Task CreateAsync(NoteModel note, CancellationToken cancellationToken = default);
        Task<NoteModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
        Task<NoteModel?> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default);
        Task DeleteAsync(NoteModel note, CancellationToken cancellationToken = default);
        Task<List<NoteModel>> GetAllAsync(
            Guid userId,
            int skip = 0,
            int take = FieldLimits.DefaultPageSize,
            CancellationToken cancellationToken = default);
    }
}
