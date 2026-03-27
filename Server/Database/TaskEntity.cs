namespace Server.DataBase
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public int ColumnId { get; set; }
        // внешний ключ 
        public int UserId { get; set; }
        // навигация
        public UserEntity User { get; set; }
        public List<LabelEntity> Labels { get; set; } = new();
    }
}