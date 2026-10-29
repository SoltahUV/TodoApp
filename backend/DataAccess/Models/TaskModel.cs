namespace DataAccess.Models
{
    public class TaskModel
    {
        public Guid UserId { get; init; }
        public Guid ItemId { get; init; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskProgressStatus Status { get; set; } = TaskProgressStatus.NotStarted;
        public DateTime Created { get; init; }
        public DateTime Updated { get; set; }
    }
    public enum TaskProgressStatus
    {
        NotStarted,
        InProgress,
        Done
    }
}
