using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using AvaloniaClient.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
    public sealed class ColumnViewModel : ViewModelBase, IRoutableViewModel
    {
        public string? UrlPathSegment => "board";
        public IScreen HostScreen { get; }
        
        private CancellationTokenSource? _cts;

        private readonly IColumnService _columnService;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly ILabelService _labelService;
        private readonly ITeamService _teamService;


        // задачи

        private AddTaskViewModel _addTask;
        public AddTaskViewModel AddTask
        {
            get => _addTask;
            set => this.RaiseAndSetIfChanged(ref _addTask, value);
        }

        private bool _isAddTaskOpen;
        public bool IsAddTaskOpen
        {
            get => _isAddTaskOpen;
            set => this.RaiseAndSetIfChanged(ref _isAddTaskOpen, value);
        }

        private bool _isEditTaskOpen;

        public EditTaskViewModel _editTask;
        public EditTaskViewModel EditTask
        {
            get => _editTask;
            set => this.RaiseAndSetIfChanged(ref _editTask, value);
        }
        public bool IsEditTaskOpen
        {
            get => _isEditTaskOpen;
            set => this.RaiseAndSetIfChanged(ref _isEditTaskOpen, value);
        }

        // колонки

        private AddColumnViewModel _addColumn;
        public AddColumnViewModel AddColumn
        {
            get => _addColumn;
            set => this.RaiseAndSetIfChanged(ref _addColumn, value);
        }

        private bool _isAddColumnOpen;
        public bool IsAddColumnOpen
        {
            get => _isAddColumnOpen;
            set => this.RaiseAndSetIfChanged(ref _isAddColumnOpen, value);
        }

        private EditColumnViewModel _editColumn;
        public EditColumnViewModel EditColumn
        {
            get => _editColumn;
            set => this.RaiseAndSetIfChanged(ref _editColumn, value);
        }
        public bool IsEditColumnOpen
        {
            get => _isEditColumnOpen;
            set => this.RaiseAndSetIfChanged(ref _isEditColumnOpen, value);
        }
        private bool _isEditColumnOpen;

        public ObservableCollection<ColumnModel> Columns { get; } = new();
        public ObservableCollection<TeamModel> Teams { get; } = new();
        private IReadOnlyList<TaskModel> Tasks = new List<TaskModel>();

        private bool IsInitialized;
        public ReactiveCommand<Unit, Task> NavigateToSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenAddColumnDialogCommand { get; }
        public ReactiveCommand<ColumnModel, Unit> OpenEditColumnDialogCommand { get; }
        public ReactiveCommand<ColumnModel, Unit> EditColumnCommand { get; }

        public ReactiveCommand<int, Unit> OpenAddTaskDialogCommand { get; }
        public ReactiveCommand<TaskModel, Unit> OpenEditTaskDialogCommand { get; }
        public ReactiveCommand<TaskModel, Unit> EditTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> DeleteTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> CompleteTaskCommand { get; }


        private DateTime _now = DateTime.Now;
        public DateTime Now
        {
            get => _now;
            set => this.RaiseAndSetIfChanged(ref _now, value);
        }
        public NavigationService _navigationService;
        private readonly SignalRService _signalRService;
        public ColumnViewModel(SignalRService signalRService, IColumnService columnService, ITaskService taskService, IUserService userService, ILabelService labelService, ITeamService teamService, NavigationService navigationService, IScreen screen)
        {
            _columnService = columnService;
            _taskService = taskService;
            _userService = userService;
            _labelService = labelService;
            _teamService = teamService;
            _navigationService = navigationService;
            HostScreen = screen;

            _signalRService = signalRService;

            _signalRService.TaskUpdated += OnTaskUpdatedFromServer;
            _signalRService.TaskCreated += OnTaskCreatedFromServer;
            _signalRService.TaskDeleted += OnTaskDeletedFromServer;
            _signalRService.ColumnUpdated += OnColumnUpdatedFromServer;
            _signalRService.ColumnCreated += OnColumnCreatedFromServer;
            _signalRService.ColumnDeleted += OnColumnDeletedFromServer;

            NavigateToSettingsCommand = ReactiveCommand.Create(NavigateToSettingsAsync);
            OpenAddColumnDialogCommand = ReactiveCommand.Create(OpenAddColumnDialog);
            OpenAddTaskDialogCommand = ReactiveCommand.CreateFromTask<int>(OpenAddTaskDialogAsync);
            EditTaskCommand = ReactiveCommand.Create<TaskModel>(OpenEditTaskDialog);
            EditColumnCommand = ReactiveCommand.Create<ColumnModel>(OpenEditColumnDialog);
            CompleteTaskCommand = ReactiveCommand.CreateFromTask<TaskModel>(CompleteTaskAsync);

            Observable.Interval(TimeSpan.FromSeconds(1))
               .ObserveOn(RxApp.MainThreadScheduler)
               .Subscribe(_ =>
               {
                   Now = DateTime.Now;

                   foreach (var column in Columns)
                   {
                       foreach (var task in column.Tasks)
                       {
                           task.TimeLeft = GetTimeLeft(task);
                           task.OverDeadline = IsOverDeadline(task);

                           task.RaisePropertyChanged(nameof(TaskModel.DeadlineBorderBrush));
                           task.RaisePropertyChanged(nameof(TaskModel.DeadlineBorderThickness));

                       }
                   }
               });
        }


        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                var token = _cts.Token;
                await _signalRService.StopAsync();
                await _signalRService.StartAsync(token);
                await ReloadDataAsync(token);
                await LoadColumnsAsync(token);
                IsInitialized = true;
            }
        }

        private async Task ReloadDataAsync(CancellationToken token)
        {
            Tasks = await _taskService.GetTasksAsync(token) ?? new List<TaskModel>();
        }

        private async Task LoadColumnsAsync(CancellationToken token)
        {
           
            var columns = await _columnService.GetColumnsAsync(token);
            var teams = await _teamService.GetTeamsAsync(token);

            foreach (var column in columns ?? new())
            {
                Columns.Add(column);
            }

            foreach (var team in teams ?? new())
            {
                Teams.Add(team);
            }

            foreach (var task in Tasks)
            {
                task.Team = Teams.FirstOrDefault(t => task.TeamIds.Contains(t.Id));

                Columns[task.ColumnId - 1].Tasks.Add(task);
            }
        }
        #region Подписки на SignalR
        private async void OnTaskUpdatedFromServer(TaskModel task)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var existing = Columns.SelectMany(c => c.Tasks).FirstOrDefault(t => t.Id == task.Id);
                if (existing != null)
                {
                    existing.Title = task.Title;
                    existing.Description = task.Description;
                    existing.ColumnId = task.ColumnId;
                    existing.Deadline = task.Deadline;
                    existing.TimeLeft = GetTimeLeft(existing);
                    existing.OverDeadline = IsOverDeadline(existing);
                    existing.Color = task.Color;
                    existing.Position = task.Position;
                    existing.Labels = task.Labels;
                    existing.TeamIds = task.TeamIds;
                    existing.Team = task.Team;
                }
            });
        }
        private async void OnTaskCreatedFromServer(TaskModel task)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                task.Team = Teams.FirstOrDefault(t => task.TeamIds.Contains(t.Id));

                Columns[task.ColumnId - 1].Tasks.Add(task);
            });
        }
        private async void OnTaskDeletedFromServer(int id)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (var column in Columns ?? new())
                {
        
                    var existing = column.Tasks.FirstOrDefault(t => t.Id == id);
                    if (existing != null)
                    {
                        column.Tasks.Remove(existing);
                        return;
                    }
                }
            });
        }

        private async void OnColumnUpdatedFromServer(ColumnModel column)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var existing = Columns.FirstOrDefault(t => t.Id == column.Id);
                if (existing != null)
                {
                    existing.Title = column.Title;
                }
            });
        }
        private async void OnColumnCreatedFromServer(ColumnModel column)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Columns.Add(column);
            });
        }
        private async void OnColumnDeletedFromServer(int id)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var existing = Columns.FirstOrDefault(u => u.Id == id);
                if (existing != null)
                    Columns.Remove(existing);
            });
        }

        #endregion

        public string GetTimeLeft(TaskModel task)
        {
            if (task.IsCompleted)
                return "Выполнено"; 

            if (!task.Deadline.HasValue)
                return "Без дедлайна";

            var deadline = task.Deadline.Value.Kind == DateTimeKind.Utc ? task.Deadline.Value.ToLocalTime() : task.Deadline.Value;
            var time = deadline - Now;

            if (time <= TimeSpan.Zero)
                return "Просрочено";

            // годы
            if (time.TotalDays >= 365)
            {
                var years = (int)(time.TotalDays / 365);
                var months = (int)((time.TotalDays % 365) / 30);
                return months > 0 ? $"{years} г. {months} мес." : $"{years} г.";
            }

            // месяцы
            if (time.TotalDays >= 30)
            {
                var months = (int)(time.TotalDays / 30);
                var days = (int)(time.TotalDays % 30);
                return days > 0 ? $"{months} мес. {days} д." : $"{months} мес.";
            }

            // дни
            if (time.TotalDays >= 1)
            {
                return time.Hours > 0 ? $"{time.Days} д. {time.Hours} ч." : $"{time.Days} д.";
            }

            // часы
            if (time.TotalHours >= 1)
            {
                return $"{time.Hours} ч. {time.Minutes} мин.";
            }

            // минуты
            if (time.TotalMinutes >= 1)
            {
                return $"{time.Minutes} мин.";
            }

            return "Меньше минуты";
        }

        // 24 часа до дедлайна
        public bool IsOverDeadline(TaskModel task)
        {
            if (task.IsCompleted)
                return false;

            if (!task.Deadline.HasValue)
                return false;

            var deadline = task.Deadline.Value;

            if (deadline.Kind == DateTimeKind.Utc)
                deadline = deadline.ToLocalTime();

            var timeLeft = deadline - Now;

            return timeLeft <= TimeSpan.FromHours(24);

        }

        #region Колонки
        private void OpenAddColumnDialog()
        {
            AddColumn = new AddColumnViewModel();
            IsAddColumnOpen = true;

            AddColumn.SaveCommand.Subscribe(dto =>
            {
                if (dto != null)
                {
                    CreateColumn(dto);
                }
            });

            AddColumn.CancelCommand.Subscribe(_ =>
            {
                IsAddColumnOpen = false;
            });
        }
        private void OpenEditColumnDialog(ColumnModel column)
        {
            EditColumn = new EditColumnViewModel(column);
            IsEditColumnOpen = true;

            EditColumn.SaveCommand.Subscribe(async updatedColumn =>
            {
                if (updatedColumn == null)
                    return;

                try
                {

                    IsEditColumnOpen = false;
                    await _columnService.UpdateColumnAsync(updatedColumn);

                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            });

            EditColumn.DeleteCommand.Subscribe(async deleteColumn =>
            {
                if (deleteColumn == null)
                    return;

                IsEditColumnOpen = false;

                await _columnService.DeleteColumnAsync(deleteColumn);

                var column = Columns.FirstOrDefault(c => c.Id == deleteColumn);
                Columns.Remove(column);
            });

            EditColumn.CancelCommand.Subscribe(_ =>
            {
                IsEditColumnOpen = false;
            });

        }
        
        private async Task CreateColumn(ColumnDTO dto)
        {
            try
            {
                IsAddColumnOpen = false;

                var created = await _columnService.CreateColumnAsync(dto);

                /* if (created != null)
                 {
                     Columns.Add(new ColumnModel
                     {
                         Id = created.Id,
                         Title = created.Title,
                         Tasks = new ObservableCollection<TaskModel>()
                     });
                 }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Задачи


        private async Task OpenAddTaskDialogAsync(int columnId)
        {
            AddTask = new AddTaskViewModel(columnId, _navigationService, _userService, _labelService, _teamService, _columnService);
            IsAddTaskOpen = true;

            AddTask.SaveCommand.Subscribe(task =>
            {
                if (task != null)
                {
                    CreateTask(task);
                }
            });

            AddTask.CancelCommand.Subscribe(_ =>
            {
                IsAddTaskOpen = false;
            });
        }


        private async Task CompleteTaskAsync(TaskModel task)
        {
            if (task == null)
                return;

            try
            {
                task.IsCompleted = true;
                task.TimeLeft = "Выполнено";

                await _taskService.UpdateTaskAsync(task);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }



        private void OpenEditTaskDialog(TaskModel task)
        {
            var oldColumnId = task.ColumnId;
            EditTask = new EditTaskViewModel(task, _userService, _labelService, _teamService, _columnService, _taskService);
            IsEditTaskOpen = true;

            EditTask.SaveCommand.Subscribe(async updatedTask =>
            {
                if (updatedTask == null)
                    return;

                try
                {
                    IsEditTaskOpen = false;
                    if (oldColumnId != updatedTask.ColumnId)
                    {
                        var newColumn = Columns.First(c => c.Id == updatedTask.ColumnId);

                        await MoveTaskToColumnAsync(updatedTask, updatedTask.ColumnId, newColumn.Tasks.Count);
                    }

                    await _taskService.UpdateTaskAsync(updatedTask);

                }
                
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            });

            EditTask.DeleteCommand.Subscribe(async deleteTask =>
            {
                if (deleteTask == null)
                    return;

                IsEditTaskOpen = false;

                await _taskService.DeleteTaskAsync(deleteTask.Id);

                //var column = Columns.FirstOrDefault(c => c.Id == deleteTask.ColumnId);
                //column?.Tasks.Remove(deleteTask);
            });



            EditTask.CancelCommand.Subscribe(_ =>
            {
                IsEditTaskOpen = false;
            });
        }


        private async Task CreateTask(TaskModel task)
        {
            try
            {
                IsAddTaskOpen = false;

                var created = await _taskService.CreateTaskAsync(task);

                /*if (created != null)
                {
                    var column = Columns.FirstOrDefault(x => x.Id == created.ColumnId);
                    column?.Tasks.Add(created);
                }*/
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        #endregion
        private async Task NavigateToSettingsAsync()
        {
            await _navigationService.NavigateToSettingsAsync();
        }

        #region Drag-and-Drop
        public void ReorderTaskInColumn(int columnId, int oldPosition, int newPosition)
        {
            var column = Columns.First(c => c.Id == columnId);

            var task = column.Tasks[oldPosition];
            column.Tasks.RemoveAt(oldPosition);
            column.Tasks.Insert(newPosition, task);

            for (int i = 0; i < column.Tasks.Count; i++)
            {
                column.Tasks[i].Position = i;
                _taskService.UpdateTaskAsync(column.Tasks[i]); 
            }
        }
        public async Task MoveTaskToColumnAsync(TaskModel task, int newColumnId, int newPosition)
        {
            var oldColumn = Columns.First(c => c.Tasks.Contains(task));
            var newColumn = Columns.First(c => c.Id == newColumnId);

            oldColumn.Tasks.Remove(task);

            for (int i = 0; i < oldColumn.Tasks.Count; i++)
                oldColumn.Tasks[i].Position = i;

            task.ColumnId = newColumnId;
            task.Position = newPosition;
            newColumn.Tasks.Insert(newPosition, task);

            for (int i = 0; i < newColumn.Tasks.Count; i++)
                newColumn.Tasks[i].Position = i;

            await _taskService.UpdateTaskAsync(task);
            await _taskService.AddHistoryEntryAsync(
                task.Id,
                "Перемещение",
                oldColumn.Title,
                newColumn.Title,
                $"Задача \"{task.Title}\" перемещена из колонки \"{oldColumn.Title}\" в колонку \"{newColumn.Title}\""
            );
        }
        public async Task ReorderColumnAsync(ColumnModel draggedColumn, ColumnModel targetColumn)
        {
            var oldIndex = Columns.IndexOf(draggedColumn);
            var newIndex = Columns.IndexOf(targetColumn);

            if (oldIndex == -1 || newIndex == -1) return;

            Columns.Move(oldIndex, newIndex);

            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].Position = i;
                _columnService.UpdateColumnAsync(Columns[i]);
            }
        }
        public async Task ReorderColumnAsync(ColumnModel draggedColumn, int newIndex)
        {
            var oldIndex = Columns.IndexOf(draggedColumn);

            if (oldIndex == -1 || newIndex == -1) return;

            Columns.Move(oldIndex, newIndex);

            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].Position = i;
                await _columnService.UpdateColumnAsync(Columns[i]);
            }
        }
        #endregion
    }
}