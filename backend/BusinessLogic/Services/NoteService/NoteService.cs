using BusinessLogic.Dtos;
using DataAccess.Data.NoteRepo;
using DataAccess.Models;


namespace BusinessLogic.Services.NoteService
{
    internal class NoteService(INoteRepository NoteRepository) : INoteService
    {
        public async Task CreateAsync(Guid userId, CreateNoteDto dto, CancellationToken cancellationToken = default)
        {
            var Note = new NoteModel()
            {
                UserId = userId,
                Name = dto.Name,
                Text = dto.Text,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            await NoteRepository.CreateAsync(Note, cancellationToken);
        }

        public async Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var note = await NoteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new Exception("Note not found");

            await NoteRepository.DeleteAsync(note, cancellationToken);
        }

        public async Task<NoteModel> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var note = await NoteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new Exception("Note not found");

            return note;
        }
        public async Task PatchByIdAsync(Guid userId, Guid itemId, PatchNoteDto dto, CancellationToken cancellationToken = default)
        {
            var note = await NoteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new Exception("Note not found");

            if (dto.Name != null)
                note.Name = dto.Name;

            if (dto.Text != null)
                note.Text = dto.Text;

            note.Updated = DateTime.UtcNow;

            await NoteRepository.UpdateAsync(note, cancellationToken);
        }
        public async Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateNoteDto dto, CancellationToken cancellationToken = default)
        {
            var note = await NoteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new Exception("Note not found");

            note.Name = dto.Name;
            note.Text = dto.Text;
            note.Updated = DateTime.UtcNow;

            await NoteRepository.UpdateAsync(note, cancellationToken);
        }
        public async Task<List<NoteModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await NoteRepository.GetAllAsync(userId, cancellationToken);
        }
    }
}
