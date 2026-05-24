using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Server.Database.Entities
{
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Color { get; set; }
        public int ColumnId { get; set; }
        public List<UserEntity?> Users { get; set; }
        public List<TeamEntity?> Teams { get; set; } = new();  
        public List<LabelEntity?> Labels { get; set; } = new();
        public ColumnEntity? Column { get; set; }
        public int Position { get; set; }
        public bool IsCompleted { get; set; }

    }
}