using Avalonia.Media;
using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;



namespace AvaloniaClient.ViewModels
{
    public sealed class AddTaskViewModel : ReactiveObject
    {
        public NavigationService _navigationService;
        private readonly IUserService _userService;
        private readonly ILabelService _labelService;
        private readonly ITeamService _teamService;
        private readonly IColumnService _columnService;

        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _columnId;
        private DateTimeOffset? _date;
        private TimeSpan? _time;
        private DateTime? _deadline;
        private string _deadlineInput = string.Empty;


        public ObservableCollection<UserModel> Users { get; } = new();
        public ObservableCollection<LabelModel> Labels { get; } = new();
        public ObservableCollection<TeamModel> Teams { get; } = new();
        public ObservableCollection<ColumnModel> Columns { get; } = new();

        private UserModel? _selectedUser;
        public UserModel? SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        private LabelModel? _selectedLabel;
        public LabelModel? SelectedLabel
        {
            get => _selectedLabel;
            set => this.RaiseAndSetIfChanged(ref _selectedLabel, value);
        }

        private TeamModel? _selectedTeam;
        public TeamModel? SelectedTeam
        {
            get => _selectedTeam;
            set => this.RaiseAndSetIfChanged(ref _selectedTeam, value);
        }

        private ColumnModel? _selectedColumnStatus;
        public ColumnModel? SelectedColumnStatus
        {
            get => _selectedColumnStatus;
            set => this.RaiseAndSetIfChanged(ref _selectedColumnStatus, value);
        }


        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public DateTime? Deadline
        {
            get => _deadline;
            set => this.RaiseAndSetIfChanged(ref _deadline, value);
        }
        public DateTimeOffset? Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }
        public TimeSpan? Time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }

        public string DeadlineInput
        {
            get => _deadlineInput;
            set
            {
                this.RaiseAndSetIfChanged(ref _deadlineInput, value);
                DataTimeParse();
            }
        }
        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
        }

        private void DataTimeParse()
        {
            DeadlineInput = Date.ToString() + Time;

            if (!string.IsNullOrWhiteSpace(DeadlineInput))
            {
                if (DateTime.TryParseExact(DeadlineInput, new[] { "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH.mm" }, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dataParse))
                {
                    Deadline = DateTime.SpecifyKind(dataParse, DateTimeKind.Local);
                }
                else
                {
                    Deadline = null;
                }
            }
            else
            {
                Deadline = null;
            }


        }

        public ReactiveCommand<Unit, TaskModel?> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Task> NavigateToSettingsCommand { get; }

        public AddTaskViewModel(int columnId, NavigationService navigationService, IUserService userService, ILabelService labelService, ITeamService teamService, IColumnService columnService)
        {
            _columnId = columnId;
            _navigationService = navigationService;
            _userService = userService;
            _labelService = labelService;
            _teamService = teamService;
            _columnService = columnService;

            _ = LoadAsync();

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    return (TaskModel?)new TaskModel
                    {
                        Title = Title,
                        Description = Description,
                        Deadline = Deadline,
                        ColumnId = SelectedColumnStatus?.Id ?? _columnId,
                        Color = SelectedColor.ToString(),
                        UserIds = SelectedUser != null ? new List<int?> { SelectedUser.Id } : new(),
                        LabelIds = SelectedLabel != null ? new List<int?> { SelectedLabel.Id } : new(),
                        TeamIds = SelectedTeam != null ? new List<int?> { SelectedTeam.Id } : new(),
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            });

            CancelCommand = ReactiveCommand.Create(() => { });

            NavigateToSettingsCommand = ReactiveCommand.Create(NavigateToSettingsAsync);
        }
        private async Task NavigateToSettingsAsync()
        {
            await _navigationService.NavigateToSettingsAsync();
        }

        public async Task LoadAsync()
        {
            var users = await _userService.GetUsersAsync();
            var labels = await _labelService.GetLabelsAsync();
            var teams = await _teamService.GetTeamsAsync();
            var columns = await _columnService.GetColumnsAsync();


            foreach (var user in users ?? new())
                Users.Add(user);

            foreach (var label in labels ?? new())
                Labels.Add(label);

            foreach (var team in teams ?? new())
                Teams.Add(team);

            foreach (var column in columns ?? new())
                Columns.Add(column);
        }
    }
}