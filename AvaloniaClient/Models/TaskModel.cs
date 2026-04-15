using System.Collections.Generic;
using System.Data;
using System;
using Avalonia.Media;
namespace AvaloniaClient.DataBase
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
        public List<int?> UserIds { get; set; }
        public List<int?> TeamIds { get; set; }
        public List<int?> LabelIds { get; set; }
        public string? Color { get; set; }
    }
}