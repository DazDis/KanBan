using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaClient.DataBase;
using AvaloniaClient.Services;

namespace AvaloniaClient.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IRoutableViewModel
    {
        private string _message;

        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ILabelService _labelService;
        private readonly NavigationService _navigationService;
        private readonly SignalRService _signalRService;

        private ObservableCollection<UserModel> _users = new();
        private ObservableCollection<TeamModel> _teams = new();
        private ObservableCollection<LabelModel> _labels = new();

        private UserModel? _selectedUser;
        private TeamModel? _selectedTeam;
        private LabelModel? _selectedLabel;

        public string? UrlPathSegment => "settings";
        public IScreen HostScreen { get; }
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
        public ObservableCollection<UserModel> Users
        {
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }
        public ObservableCollection<TeamModel> Teams
        {
            get => _teams;
            set => this.RaiseAndSetIfChanged(ref _teams, value);
        }
        public ObservableCollection<LabelModel> Labels
        {
            get => _labels;
            set => this.RaiseAndSetIfChanged(ref _labels, value);
        }

        public UserModel? SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }
        public TeamModel? SelectedTeam
        {
            get => _selectedTeam;
            set => this.RaiseAndSetIfChanged(ref _selectedTeam, value);
        }
        public LabelModel? SelectedLabel
        {
            get => _selectedLabel;
            set => this.RaiseAndSetIfChanged(ref _selectedLabel, value);
        }

        public ReactiveCommand<Unit, Unit> NavigateToColumnCommand { get; }
        public ReactiveCommand<Unit, Unit> DropDBCommand { get; }

        public ReactiveCommand<Unit, Unit> AddUserCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteUserCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAllUsersCommand { get; }

        public ReactiveCommand<Unit, Unit> AddTeamCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteTeamCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAllTeamsCommand { get; }

        public ReactiveCommand<Unit, Unit> AddLabelCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteLabelCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAllLabelsCommand { get; }
        public bool IsInitialized { get; private set; }

        public SettingsViewModel(
            IScreen screen,
            IUserService userService,
            ITeamService teamService,
            ILabelService labelService,
            NavigationService navigationService,
            SignalRService signalRService)
        {
            HostScreen = screen;
            _userService = userService;
            _teamService = teamService;
            _labelService = labelService;
            _navigationService = navigationService;
            _signalRService = signalRService;

            NavigateToColumnCommand = ReactiveCommand.CreateFromTask(NavigateToColumnAsync);
            DropDBCommand = ReactiveCommand.CreateFromTask(DropDatabaseAsync);

            AddUserCommand = ReactiveCommand.CreateFromTask(AddUserAsync);
            DeleteUserCommand = ReactiveCommand.CreateFromTask(DeleteUserAsync);
            SaveAllUsersCommand = ReactiveCommand.CreateFromTask(SaveAllUsersAsync);

            AddTeamCommand = ReactiveCommand.CreateFromTask(AddTeamAsync);
            DeleteTeamCommand = ReactiveCommand.CreateFromTask(DeleteTeamAsync);
            SaveAllTeamsCommand = ReactiveCommand.CreateFromTask(SaveAllTeamsAsync);

            AddLabelCommand = ReactiveCommand.CreateFromTask(AddLabelAsync);
            DeleteLabelCommand = ReactiveCommand.CreateFromTask(DeleteLabelAsync);
            SaveAllLabelsCommand = ReactiveCommand.CreateFromTask(SaveAllLabelsAsync);

            _signalRService.LabelUpdated += OnLabelUpdatedFromServer;
            _signalRService.LabelCreated += OnLabelCreatedFromServer;
            _signalRService.LabelDeleted += OnLabelDeletedFromServer;
            _signalRService.TeamUpdated += OnTeamUpdatedFromServer;
            _signalRService.TeamCreated += OnTeamCreatedFromServer;
            _signalRService.TeamDeleted += OnTeamDeletedFromServer;
            _signalRService.UserUpdated += OnUserUpdatedFromServer;
            _signalRService.UserCreated += OnUserCreatedFromServer;
            _signalRService.UserDeleted += OnUserDeletedFromServer;
        }

        private void OnUserDeletedFromServer(int id)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var existing = Users.FirstOrDefault(u => u.Id == id);
                if (existing != null)
                    Users.Remove(existing);
            });
        }

        private void OnTeamDeletedFromServer(int id)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var existing = Teams.FirstOrDefault(u => u.Id == id);
                if (existing != null)
                    Teams.Remove(existing);
            });
        }

        private void OnLabelDeletedFromServer(int id)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var existing = Labels.FirstOrDefault(u => u.Id == id);
                if (existing != null)
                    Labels.Remove(existing);
            });
        }

        private void OnUserCreatedFromServer(UserModel model)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (!Users.Any(u => u.Id == model.Id))
                {
                    Users.Add(model);
                }
            });
        }

        private void OnUserUpdatedFromServer(UserModel model)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var existing = Users.FirstOrDefault(u => u.Id == model.Id);
                if (existing != null)
                {
                    existing.FirstName = model.FirstName;
                    existing.LastName = model.LastName;
                    existing.Email = model.Email;
                }
                else
                {
                    Users.Add(model);
                }
            });
        }

        private void OnTeamCreatedFromServer(TeamModel model)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (!Teams.Any(t => t.Id == model.Id))
                {
                    Teams.Add(model);
                }
            });
        }

        private void OnTeamUpdatedFromServer(TeamModel model)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var existing = Teams.FirstOrDefault(t => t.Id == model.Id);
                if (existing != null)
                {
                    existing.Title = model.Title;
                }
                else
                {
                    Teams.Add(model);
                }
            });
        }

        private void OnLabelCreatedFromServer(LabelModel model)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (!Labels.Any(l => l.Id == model.Id))
                {
                    Labels.Add(model);
                }
            });
        }

        private void OnLabelUpdatedFromServer(LabelModel model)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var existing = Labels.FirstOrDefault(l => l.Id == model.Id);
                if (existing != null)
                {
                    existing.Name = model.Name;
                    existing.Color = model.Color;
                }
                else
                {
                    Labels.Add(model);
                }
            });
        }

        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                await LoadDataAsync();
                IsInitialized = true;
            }
        }
        private async Task LoadDataAsync()
        {
            try {
                await _signalRService.StopAsync();
                await _signalRService.StartAsync();
                Users = new ObservableCollection<UserModel>(await _userService.GetUsersAsync());
                Teams = new ObservableCollection<TeamModel>(await _teamService.GetTeamsAsync());
                foreach (var team in Teams)
                {
                    var teamUsers = Users.Where(u => team.UserIds.Contains(u.Id)).ToList();
                    foreach (var user in teamUsers)
                    {
                        team.Users.Add(user);
                    }
                }
                Labels = new ObservableCollection<LabelModel>(await _labelService.GetLabelsAsync());
            }
            catch
            {
                Message = "Не удалось загрузить таблицы";
            }
        }

        // ========== ПОЛЬЗОВАТЕЛИ ==========
        private async Task AddUserAsync()
        {
            try { 
                var newUser = new UserModel { FirstName = "Новый", LastName = "Пользователь", Email = "user@example.com" };
                var created = await _userService.CreateUserAsync(newUser);
            }
            catch
            {
                Message = "Не удалось добавить пользователя";
            }
        }

        private async Task DeleteUserAsync()
        {
            try
            {
                if (SelectedUser == null) return;
                await _userService.DeleteUserAsync(SelectedUser.Id);
                Users.Remove(SelectedUser);
                SelectedUser = Users.FirstOrDefault();
            }
            catch
            {
                Message = "Не удалось удалить пользователя";
            }
        }

        private async Task SaveAllUsersAsync()
        {
            try { 
            foreach (var user in Users)
                await _userService.UpdateUserAsync(user);
            }
            catch
            {
                Message = "Не удалось обновить пользователя";
            }
        }

        // ========== КОМАНДЫ ==========
        private async Task AddTeamAsync()
        {
            try
            {
                var newTeam = new TeamModel { Title = "Новая команда" };
                var created = await _teamService.CreateTeamAsync(newTeam);
            }
            catch
            {
                Message = "Не удалось создать команду";
            }
        }

        private async Task DeleteTeamAsync()
        {
            try
            {
                if (SelectedTeam == null) return;
                await _teamService.DeleteTeamAsync(SelectedTeam.Id);
                Teams.Remove(SelectedTeam);
                SelectedTeam = Teams.FirstOrDefault();
            }
            catch
            {
                Message = "Не удалось удалить команду";
            }
        }

        private async Task SaveAllTeamsAsync()
        {
            try
            {
                foreach (var team in Teams)
                    await _teamService.UpdateTeamAsync(team);
            }
            catch
            {
                Message = "Не удалось обновить команду";
            }
        }

        // ========== ТЕГИ ==========
        private async Task AddLabelAsync()
        {
            try { 
                var newLabel = new LabelModel { Name = "Новый тег", Color = "#CCCCCC" };
                var created = await _labelService.CreateLabelAsync(newLabel);
            }
            catch
            {
                Message = "Не удалось добавить тэг";
            }
        }

        private async Task DeleteLabelAsync()
        {
            try {
                if (SelectedLabel == null) return;
                await _labelService.DeleteLabelAsync(SelectedLabel.Id);
                Labels.Remove(SelectedLabel);
                SelectedLabel = Labels.FirstOrDefault();
            }
            catch
            {
                Message = "Не удалось удалить тэг";
            }
        }

        private async Task SaveAllLabelsAsync()
        {
            try 
            { 
                foreach (var label in Labels)
                    await _labelService.UpdateLabelAsync(label);
            }
            catch
            {
                Message = "Не удалось обновить тэг";
            }
        }
        public void AddUserToTeam(UserModel user, TeamModel team)
        {
            if (team.Users.Any(u => u.Id == user.Id))
            {
                RemoveUserFromTeam(user,team);
                return;
            }

            team.Users.Add(user);

            if (!team.UserIds.Contains(user.Id))
                team.UserIds.Add(user.Id);
            _ = _teamService.UpdateTeamAsync(team);

        }
        public void RemoveUserFromTeam(UserModel user, TeamModel team)
        {
            if (!team.Users.Contains(user))
                return;
            team.Users.Remove(user);

            if (team.UserIds.Contains(user.Id))
                team.UserIds.Remove(user.Id);

            _ = _teamService.UpdateTeamAsync(team);
            this.RaisePropertyChanged(nameof(Teams));
        }
        // ========== НАВИГАЦИЯ ==========
        private async Task NavigateToColumnAsync() => await _navigationService.NavigateToColumnAsync();
        private async Task DropDatabaseAsync() { /* логика удаления БД */ }
    }
}