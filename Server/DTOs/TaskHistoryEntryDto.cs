namespace Server.DTOs
{
    public class TaskHistoryEntryDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string ActionType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Comment { get; set; }
    }
}
