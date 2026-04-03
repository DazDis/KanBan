using System.ComponentModel.DataAnnotations;

namespace Server.DataBase
{
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public int ColumnId { get; set; }
        // внешний ключ 
        public List<int?> UserIds { get; set; }
        // навигация
        public List<UserEntity?> Users { get; set; }
        public List<LabelEntity?> Labels { get; set; } = new();
    }
}