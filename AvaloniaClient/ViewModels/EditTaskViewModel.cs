using Avalonia.Media;
using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
	public sealed class EditTaskViewModel : ReactiveObject

    {
        private readonly TaskModel _task;
        private readonly IUserService _userService;
        private readonly ILabelService _labelService;
        private readonly ITeamService _teamService;
        private readonly IColumnService _columnService;

        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _columnId;
        private string _selectedColor;
        private DateTimeOffset? _date;
        private TimeSpan? _time;
        private DateTime? _deadline;

        public ObservableCollection<UserModel> Users { get; } = new();
        public ObservableCollection<LabelModel> Labels { get; } = new();

        public ObservableCollection<ColumnModel> Columns { get; } = new();

        public ReactiveCommand<Unit, TaskModel?> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, TaskModel> DeleteCommand { get; }
        public ObservableCollection<TeamModel> Teams { get; } = new();

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
            set
            {
                this.RaiseAndSetIfChanged(ref _date, value);
                DataTimeParse();
            }
        }
        public TimeSpan? Time
        {
            get => _time;
            set
            {
                this.RaiseAndSetIfChanged(ref _time, value);
                DataTimeParse();
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
        }

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

        private void DataTimeParse()
        {
            if (Date.HasValue && Time.HasValue)
            {
                Deadline = Date.Value.Date + Time.Value;
            }
            else if (Date.HasValue)
            {
                Deadline = Date.Value.Date;
            }
            else
            {
                Deadline = null;
            }
        }

        public EditTaskViewModel(TaskModel task, IUserService userService, ILabelService labelService, ITeamService teamService, IColumnService columnService)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));

            Title = _task.Title;
            Description = _task.Description;
            Deadline = _task.Deadline;
            _columnId = _task.ColumnId;
            _selectedColor = _task.Color;
            _userService = userService;
            _labelService = labelService;
            _teamService = teamService;
            _columnService = columnService;

            _ = LoadAsync();
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    _task.Title = Title;
                    _task.Description = Description;
                    _task.ColumnId = SelectedColumnStatus?.Id ?? _columnId;
                    _task.Deadline = Deadline;
                    _task.Color = SelectedColor.ToString();

                    _task.UserIds = SelectedUser != null ? new List<int?> { SelectedUser.Id } : new();

                    _task.LabelIds = SelectedLabel != null ? new List<int?> { SelectedLabel.Id } : new();

                    _task.TeamIds = SelectedTeam != null ? new List<int?> { SelectedTeam.Id } : new();

                    _task.Labels = SelectedLabel != null ? new ObservableCollection<LabelModel> { SelectedLabel } : new();

                    return _task;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            });

            DeleteCommand = ReactiveCommand.Create(() => _task);

            CancelCommand = ReactiveCommand.Create(() => { });
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

            foreach(var column in columns ?? new())
                Columns.Add(column);

            //if (_task.UserIds?.Count > 0)
            //    SelectedUser = Users.FirstOrDefault(x => x.Id == _task.UserIds[0]);

            //if (_task.LabelIds?.Count > 0)
            //    SelectedLabel = Labels.FirstOrDefault(x => x.Id == _task.LabelIds[0]);

            //if (_task.TeamIds?.Count > 0)
            //    SelectedTeam = Teams.FirstOrDefault(x => x.Id == _task.TeamIds[0]);

            //SelectedColumnStatus =  Columns.FirstOrDefault(x => x.Id == _task.ColumnId);
        }
    }
}