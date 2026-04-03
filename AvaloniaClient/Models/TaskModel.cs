using System.Collections.Generic;

namespace AvaloniaClient.DataBase
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ColumnId { get; set; }
        public List<int?> UserIds { get; set; }
        public List<int?> LabelIds { get; set; }
    }
}