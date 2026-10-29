namespace DataAccess.Models
{
    public class NoteModel
    {
        public Guid UserId { get; init; }
        public Guid ItemId { get; init; }
        public string Name { get; set; } = string.Empty;
        public string? Text { get; set; }
        public DateTime Created { get; init; }
        public DateTime Updated { get; set; }
    }
}
