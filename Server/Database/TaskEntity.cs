namespace Server.DataBase
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public int ColumnId { get; set; }
        public List<UserEntity> Users { get; set; } = new();
        public List<LabelEntity> Labels { get; set; } = new();
    }
}