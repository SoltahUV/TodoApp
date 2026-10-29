using BusinessLogic.Dto;
using DataAccess.Models;

namespace BusinessLogic.Mapping;

internal static class NoteMappings
{
    public static NoteDto ToDto(this NoteModel note) =>
        new(note.ItemId, note.Name, note.Text, note.Created, note.Updated);
}
