using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Data.NoteRepo
{
    internal class NoteRepository(AppDbContext context, ILogger<NoteRepository> logger) : INoteRepository
    {

        public async Task CreateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            await context.Notes.AddAsync(note, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created note for user {UserId} and item {ItemId}", note.UserId, note.ItemId);
        }

        public async Task<NoteModel?> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var note = await context.Notes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ItemId == itemId, cancellationToken);
            logger.LogInformation("Retrieved note for user {UserId} and item {ItemId}", userId, itemId);
            return note;
        }

        public async Task<NoteModel?> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Updated note for user {UserId} and item {ItemId}", note.UserId, note.ItemId);
            return note;
        }
        public async Task DeleteAsync(NoteModel note, CancellationToken cancellationToken = default)
        {
            context.Notes.Remove(note);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Deleted note for user {UserId} and item {ItemId}", note.UserId, note.ItemId);
        }
        public async Task<List<NoteModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var notes = await context.Notes
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            logger.LogInformation("Retrieved all notes for user {UserId}", userId);
            return notes;
        }
    }
}
