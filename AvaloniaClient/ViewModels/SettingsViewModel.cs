using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
	public class SettingsViewModel : ReactiveObject, IRoutableViewModel
    {
		IConfigurationService _configurationService; 
		IUserService _userService;
		IHealthService _healthService;
        ILabelService _labelService;
        NavigationService _navigationService;
        //public IReadOnlyList<UserModel> Users = new List<UserModel>();
        public ObservableCollection<UserModel> Users { get; set; } = new();
        public ObservableCollection<TeamModel> Teams { get; set; } = new();
        public ObservableCollection<LabelModel> Labels { get; } = new();
        public ReactiveCommand<Unit, Unit> AddUserCommand { get; }
        public ReactiveCommand<UserModel, Unit> DeleteUserCommand { get; }

        public ReactiveCommand<Unit, Unit> AddLabelCommand { get; }
        public ReactiveCommand<LabelModel, Unit> DeleteLabelCommand { get; }
        private UserModel _selectedUser;
        public UserModel SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }




        private LabelModel _selectedLabel;
        public LabelModel SelectedLabel
        {
            get => _selectedLabel;
            set => this.RaiseAndSetIfChanged(ref _selectedLabel, value);
        }
        public TeamModel SelectedTeam { get; set; }

        private bool IsInitialized;

        private int _id;
        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public class TeamModel : ReactiveObject
        {
            public string Title { get; set; }
            public string ColorTeam { get; set; }

            public ObservableCollection<UserModel> Users { get; set; } = new();

            private UserModel _selectedUserToAdd;
            public UserModel SelectedUserToAdd
            {
                get => _selectedUserToAdd;
                set => this.RaiseAndSetIfChanged(ref _selectedUserToAdd, value);
            }

            public ReactiveCommand<Unit, Unit> AddUserToTeamCommand { get; }

            public TeamModel()
            {
                AddUserToTeamCommand = ReactiveCommand.Create(() =>
                {
                    if (SelectedUserToAdd != null && !Users.Contains(SelectedUserToAdd))
                        Users.Add(SelectedUserToAdd);
                });
            }
        }
        public string? UrlPathSegment => "settings";
        public string UrlServerPath {
            get;
            set => _configurationService.SaveApiUrl(value);
        }

        public IScreen HostScreen { get; }
        public ReactiveCommand<Unit, Task> NavigateToColumnCommand { get; }
        public ReactiveCommand<Unit, Task> DropDBCommand { get; }
        public ReactiveCommand<UserModel, Unit> CreateUserCommand { get; }
        public SettingsViewModel( IConfigurationService configurationService, IHealthService healthService ,IUserService userService, ILabelService labelService, IScreen screen, NavigationService navigationService) 
		{
			_configurationService = configurationService;
			_labelService = labelService;
			_userService = userService;
            _navigationService = navigationService;
            _healthService = healthService;
            HostScreen = screen;

            NavigateToColumnCommand = ReactiveCommand.Create(NavigateToColumnAsync);
            DropDBCommand = ReactiveCommand.Create(DropDBAsync);
            CreateUserCommand = ReactiveCommand.CreateFromTask<UserModel>(CreateUserAsync);

        }

        private async Task CreateUserAsync(UserModel user)
        {
            if (user == null) return;

            var createdUser = await _userService.CreateUserAsync(user);
        }

        private async Task NavigateToColumnAsync()
        {
            await _navigationService.NavigateToColumnAsync();
        }
        private async Task DropDBAsync()
        {
            await _healthService.DropDBAsync();
        }
        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                await LoadData();
                IsInitialized = true;
            }
        }

        private async Task LoadData()
        {
            var users = await _userService.GetUsersAsync();
            var labels = await _labelService.GetLabelsAsync();

            foreach (var user in users ?? new())
            {
                Users.Add(user);
            }

            foreach (var label in labels ?? new())
            {
                Labels.Add(label);
            }
        }


    }
}