using System.Collections.ObjectModel;

namespace AvaloniaClient.DataBase
{
    public class ColumnModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<TaskModel> Tasks { get; set; } = new();
        public int Order { get; set; }

    }
}