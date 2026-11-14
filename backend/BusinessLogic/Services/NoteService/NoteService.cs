using BusinessLogic.Dto;
using BusinessLogic.Mapping;
using DataAccess.Data.NoteRepo;
using DataAccess.Models;
using Shared.Constraints;
using Shared.Exceptions;

namespace BusinessLogic.Services.NoteService
{
    internal class NoteService(INoteRepository noteRepository) : INoteService
    {
        public async Task<NoteDto> CreateAsync(Guid userId, CreateNoteDto dto, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var note = new NoteModel()
            {
                UserId = userId,
                Name = dto.Name,
                Text = dto.Text,
                Created = now,
                Updated = now
            };

            await noteRepository.CreateAsync(note, cancellationToken);

            return note.ToDto();
        }

        public async Task DeleteByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var note = await noteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new NotFoundException("Note not found");

            await noteRepository.DeleteAsync(note, cancellationToken);
        }

        public async Task<NoteDto> GetByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var note = await noteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new NotFoundException("Note not found");

            return note.ToDto();
        }

        public async Task PatchByIdAsync(Guid userId, Guid itemId, PatchNoteDto dto,
            CancellationToken cancellationToken = default)
        {
            var note = await noteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new NotFoundException("Note not found");

            if (dto.Name != null)
                note.Name = dto.Name;

            if (dto.Text != null)
                note.Text = dto.Text;

            note.Updated = DateTime.UtcNow;

            await noteRepository.UpdateAsync(note, cancellationToken);
        }

        public async Task UpdateByIdAsync(Guid userId, Guid itemId, UpdateNoteDto dto,
            CancellationToken cancellationToken = default)
        {
            var note = await noteRepository.GetByIdAsync(userId, itemId, cancellationToken);

            if (note == null)
                throw new NotFoundException("Note not found");

            note.Name = dto.Name;
            note.Text = dto.Text;
            note.Updated = DateTime.UtcNow;

            await noteRepository.UpdateAsync(note, cancellationToken);
        }

        public async Task<List<NoteDto>> GetAllAsync(
            Guid userId,
            int skip = 0,
            int take = FieldLimits.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var notes = await noteRepository.GetAllAsync(userId, skip, take, cancellationToken);

            return notes.Select(note => note.ToDto()).ToList();
        }
    }
}
