using ReactiveUI;

namespace AvaloniaClient.DataBase
{
    public class LabelModel : ReactiveObject
    {
        private int _id;
        private string _name;
        private string _color;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }
    }
}