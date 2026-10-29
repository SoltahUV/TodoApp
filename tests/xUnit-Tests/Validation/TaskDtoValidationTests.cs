using BusinessLogic.Dto;
using DataAccess.Models;
using Shared.Constraints;

namespace XUnit_Tests.Validation;

public class TaskDtoValidationTests
{
    [Fact]
    public void Create_WithNameAndDescription_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new CreateTaskDto("Task", "Description")));

    [Fact]
    public void Create_WithoutDescription_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new CreateTaskDto("Task", null)));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_BlankName_IsInvalid(string name) =>
        Assert.Contains(nameof(CreateTaskDto.Name), DtoValidator.InvalidMembers(new CreateTaskDto(name, null)));

    [Fact]
    public void Create_NameAtLimit_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(
            new CreateTaskDto(new string('a', FieldLimits.ItemNameMax), null)));

    [Fact]
    public void Create_NameOverLimit_IsInvalid() =>
        Assert.Contains(nameof(CreateTaskDto.Name), DtoValidator.InvalidMembers(
            new CreateTaskDto(new string('a', FieldLimits.ItemNameMax + 1), null)));

    [Fact]
    public void Create_DescriptionOverLimit_IsInvalid() =>
        Assert.Contains(nameof(CreateTaskDto.Description), DtoValidator.InvalidMembers(
            new CreateTaskDto("Task", new string('a', FieldLimits.TaskDescriptionMax + 1))));

    [Fact]
    public void Update_WithoutStatus_IsInvalid() =>
        Assert.Contains(nameof(UpdateTaskDto.Status), DtoValidator.InvalidMembers(
            new UpdateTaskDto("Task", null, null)));

    [Fact]
    public void Update_UndefinedStatus_IsInvalid() =>
        Assert.Contains(nameof(UpdateTaskDto.Status), DtoValidator.InvalidMembers(
            new UpdateTaskDto("Task", null, (TaskProgressStatus)99)));

    [Fact]
    public void Update_WithStatus_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(
            new UpdateTaskDto("Task", null, TaskProgressStatus.Done)));

    [Fact]
    public void Patch_WithoutFields_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new PatchTaskDto(null, null, null)));

    [Fact]
    public void Patch_BlankName_IsInvalid() =>
        Assert.Contains(nameof(PatchTaskDto.Name), DtoValidator.InvalidMembers(
            new PatchTaskDto("   ", null, null)));

    [Fact]
    public void Patch_UndefinedStatus_IsInvalid() =>
        Assert.Contains(nameof(PatchTaskDto.Status), DtoValidator.InvalidMembers(
            new PatchTaskDto(null, null, (TaskProgressStatus)99)));

    [Fact]
    public void Patch_DescriptionOverLimit_IsInvalid() =>
        Assert.Contains(nameof(PatchTaskDto.Description), DtoValidator.InvalidMembers(
            new PatchTaskDto(null, new string('a', FieldLimits.TaskDescriptionMax + 1), null)));
}
