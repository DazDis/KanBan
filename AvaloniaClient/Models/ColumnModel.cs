using ReactiveUI;
using System.Collections.ObjectModel;

namespace AvaloniaClient.DataBase
{
    public class ColumnModel : ReactiveObject
    {
        private int _id;
        private string _title;
        private ObservableCollection<TaskModel> _tasks = new();
        private int _position;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public ObservableCollection<TaskModel> Tasks
        {
            get => _tasks;
            set => this.RaiseAndSetIfChanged(ref _tasks, value);
        }

        public int Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }
    }
}