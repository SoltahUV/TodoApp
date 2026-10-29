using System.ComponentModel.DataAnnotations;
using Shared.Constraints;
using Shared.Validation;

namespace BusinessLogic.Dto
{
    public record CreateNoteDto(
        [Required, StringLength(FieldLimits.ItemNameMax)] string Name,
        [StringLength(FieldLimits.NoteTextMax)] string? Text);

    public record UpdateNoteDto(
        [Required, StringLength(FieldLimits.ItemNameMax)] string Name,
        [StringLength(FieldLimits.NoteTextMax)] string? Text);

    public record PatchNoteDto(
        [NotBlank, StringLength(FieldLimits.ItemNameMax)] string? Name,
        [StringLength(FieldLimits.NoteTextMax)] string? Text);

    public record NoteDto(
        Guid ItemId,
        string Name,
        string? Text,
        DateTime Created,
        DateTime Updated);
}
