using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Constraints;

namespace DataAccess.Data.NoteRepo
{
    internal class NoteRepository(AppDbContext context) : INoteRepository
    {

        public async Task CreateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            await context.Notes.AddAsync(note, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<NoteModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            return await context.Notes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ItemId == itemId, cancellationToken);
        }

        public async Task<NoteModel?> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
            return note;
        }
        public async Task DeleteAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            context.Notes.Remove(note);
            await context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<NoteModel>> GetAllAsync(
            Guid userId,
            int skip = 0,
            int take = FieldLimits.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var safeSkip = Math.Max(0, skip);
            var safeTake = Math.Clamp(take, 1, FieldLimits.MaxPageSize);

            return await context.Notes
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Created)
                .ThenBy(x => x.ItemId)
                .Skip(safeSkip)
                .Take(safeTake)
                .ToListAsync(cancellationToken);
        }
    }
}
