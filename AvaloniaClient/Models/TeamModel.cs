using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvaloniaClient.DataBase
{
    public class TeamModel : ReactiveObject
    {
        private int _id;
        private string _title = "";
        private string _color = "#CCCCCC";
        private List<int> _userIds = new();
        private ObservableCollection<UserModel> _users = new();
        private bool _isSelected;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public List<int> UserIds
        {
            get => _userIds;
            set => this.RaiseAndSetIfChanged(ref _userIds, value);
        }

        public ObservableCollection<UserModel?> Users
        {
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public string UsersTeam => string.Join("\n", Users.Where(u => u != null).Select(u => $"{u.FirstName} {u.LastName}"));
    }
}