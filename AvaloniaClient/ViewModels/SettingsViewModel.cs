using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
	public class SettingsViewModel : ReactiveObject, IRoutableViewModel
    {
		IConfigurationService _configurationService; 
		IUserService _userService;
		ILabelService _labelService;
        //public IReadOnlyList<UserModel> Users = new List<UserModel>();
        public ObservableCollection<UserModel> Users { get; set; } = new();
        public ObservableCollection<TeamModel> Teams { get; set; } = new();

        public UserModel SelectedUser { get; set; }

        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        public SettingsViewModel( IConfigurationService configurationService, IUserService userService, ILabelService labelService) 
		{
			_configurationService = configurationService;
			_labelService = labelService;
			_userService = userService;
		}
        public SettingsViewModel()
        {
           
        }
        public async Task InitializeAsync()
        {
            
        }
    }
}