using BusinessLogic.Dto;
using DataAccess.Models;

namespace BusinessLogic.Mapping;

internal static class TaskMappings
{
    public static TaskDto ToDto(this TaskModel task) =>
        new(task.ItemId, task.Name, task.Description, task.Status, task.Created, task.Updated);
}
