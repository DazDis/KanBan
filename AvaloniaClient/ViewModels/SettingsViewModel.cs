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

        private bool IsInitialized;

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