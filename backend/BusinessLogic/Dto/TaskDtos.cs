using DataAccess.Models;

namespace BusinessLogic.Dtos
{
    public record CreateTaskDto(string Name, string? Description);
    public record UpdateTaskDto(string Name, string Description, TaskProgressStatus Status);
    public record PatchTaskDto(string? Name, string? Description, TaskProgressStatus? Status);
}
