using BusinessLogic.Dto;
using Shared.Constraints;

namespace XUnit_Tests.Validation;

public class NoteDtoValidationTests
{
    [Fact]
    public void Create_WithoutText_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new CreateNoteDto("Note", null)));

    [Fact]
    public void Create_BlankName_IsInvalid() =>
        Assert.Contains(nameof(CreateNoteDto.Name), DtoValidator.InvalidMembers(
            new CreateNoteDto(" ", null)));

    [Fact]
    public void Create_TextOverLimit_IsInvalid() =>
        Assert.Contains(nameof(CreateNoteDto.Text), DtoValidator.InvalidMembers(
            new CreateNoteDto("Note", new string('a', FieldLimits.NoteTextMax + 1))));

    [Fact]
    public void Update_WithoutName_IsInvalid() =>
        Assert.Contains(nameof(UpdateNoteDto.Name), DtoValidator.InvalidMembers(
            new UpdateNoteDto(null!, null)));

    [Fact]
    public void Update_WithoutText_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new UpdateNoteDto("Note", null)));

    [Fact]
    public void Patch_WithoutFields_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new PatchNoteDto(null, null)));

    [Fact]
    public void Patch_TextOverLimit_IsInvalid() =>
        Assert.Contains(nameof(PatchNoteDto.Text), DtoValidator.InvalidMembers(
            new PatchNoteDto(null, new string('a', FieldLimits.NoteTextMax + 1))));
}
