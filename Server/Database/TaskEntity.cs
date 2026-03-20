namespace Server.DataBase
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public int ColumnId { get; set; }
        public List<int> UserIds { get; set; }      
        public List<int> LabelIds { get; set; }     
    }
}