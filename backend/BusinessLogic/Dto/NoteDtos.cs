namespace BusinessLogic.Dtos
{
    public record CreateNoteDto(string Name, string Text);
    public record UpdateNoteDto(string Name, string Text);
    public record PatchNoteDto(string? Name, string? Text);
}
