using System.ComponentModel.DataAnnotations;
using DataAccess.Models;
using Shared.Constraints;
using Shared.Validation;

namespace BusinessLogic.Dto
{
    public record CreateTaskDto(
        [Required, StringLength(FieldLimits.ItemNameMax)] string Name,
        [StringLength(FieldLimits.TaskDescriptionMax)] string? Description);

    public record UpdateTaskDto(
        [Required, StringLength(FieldLimits.ItemNameMax)] string Name,
        [StringLength(FieldLimits.TaskDescriptionMax)] string? Description,
        [Required, EnumDataType(typeof(TaskProgressStatus))] TaskProgressStatus? Status);

    public record PatchTaskDto(
        [NotBlank, StringLength(FieldLimits.ItemNameMax)] string? Name,
        [StringLength(FieldLimits.TaskDescriptionMax)] string? Description,
        [EnumDataType(typeof(TaskProgressStatus))] TaskProgressStatus? Status);

    public record TaskDto(
        Guid ItemId,
        string Name,
        string? Description,
        TaskProgressStatus Status,
        DateTime Created,
        DateTime Updated);
}
