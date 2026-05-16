using ReactiveUI;
using System.Collections.Generic;

namespace AvaloniaClient.DataBase
{
    public class UserModel: ReactiveObject
    {
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _email;
        private List<int> _teamIds = new();
        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public List<int> TeamIds
        {
            get => _teamIds;
            set => this.RaiseAndSetIfChanged(ref _teamIds, value);
        }
    }
}