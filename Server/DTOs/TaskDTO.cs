namespace Server.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
        public string? Color { get; set; }
        public List<int?> UserIds { get; set; }
        public List<int?> TeamIds { get; set; }
        public List<int?> LabelIds { get; set; }
        public List<LabelDTO> Labels { get; set; } = new();
        public int Position { get; set; }

    }
}