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
		ILabelService _labelService;
        NavigationService _navigationService;
        //public IReadOnlyList<UserModel> Users = new List<UserModel>();
        public ObservableCollection<UserModel> Users { get; set; } = new();
        public ObservableCollection<TeamModel> Teams { get; set; } = new();
        public ObservableCollection<LabelModel> Labels { get; } = new();

        public UserModel SelectedUser { get; set; }
        public LabelModel SelectedLabels { get; set; }
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

        public class TeamModel
        {
            public string Title { get; set; }

            public string ColorTeam { get; set; }

    
            public ObservableCollection<UserModel> Users { get; set; }
                = new ObservableCollection<UserModel>();
        }
        public string? UrlPathSegment => "settings";
        public string UrlServerPath {
            get;
            set => _configurationService.SaveApiUrl(value);
        }

        public IScreen HostScreen { get; }
        public ReactiveCommand<Unit, Task> NavigateToColumnCommand { get; }

        public SettingsViewModel( IConfigurationService configurationService, IUserService userService, ILabelService labelService, IScreen screen, NavigationService navigationService) 
		{
			_configurationService = configurationService;
			_labelService = labelService;
			_userService = userService;
            _navigationService = navigationService;
            HostScreen = screen;

            NavigateToColumnCommand = ReactiveCommand.Create(NavigateToColumnAsync);
        }

        private async Task NavigateToColumnAsync()
        {
            await _navigationService.NavigateToColumnAsync();
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