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
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
    public sealed class ColumnViewModel : ViewModelBase, IRoutableViewModel
    {
        public string? UrlPathSegment => "board";
        public IScreen HostScreen { get; }
        private readonly IColumnService _columnService;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly ILabelService _labelService;
        private readonly ITeamService _teamService;
        public ObservableCollection<ColumnModel> Columns { get; } = new();
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

            // Подписываемся на реальные обновления
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
                       }
                   }
               });
        }


        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                await _signalRService.StopAsync();
                await _signalRService.StartAsync();
                await ReloadData();
                await LoadColumns();
                IsInitialized = true;
            }
        }

        private async Task ReloadData()
        {
            Tasks = await _taskService.GetTasksAsync() ?? new List<TaskModel>();
        }

        private async Task LoadColumns()
        {
            var columns = await _columnService.GetColumnsAsync();

            foreach (var column in columns ?? new())
            {
                ColumnModel model = new ColumnModel()
                {
                    Id = column.Id,
                    Title = column.Title,
                    Tasks = new ObservableCollection<TaskModel>(),
                };

                Columns.Add(model);
            }

            foreach (var task in Tasks)
            {
                Columns[task.ColumnId - 1].Tasks.Add(task);
            }
        }
        private async void OnTaskUpdatedFromServer(TaskModel task)
        {
            // Обновляем локальный список
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var existing = Columns.SelectMany(c => c.Tasks).FirstOrDefault(t => t.Id == task.Id);
                if (existing != null)
                {
                    existing.Title = task.Title;
                    existing.Description = task.Description;
                    existing.ColumnId = task.ColumnId;
                    existing.Deadline = task.Deadline;
                    existing.Color = task.Color;
                    existing.Position = task.Position;
                }
            });
        }
        private async void OnTaskCreatedFromServer(TaskModel task)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Columns[task.ColumnId - 1].Tasks.Add(task);
            });
        }
        private async void OnTaskDeletedFromServer(int id)
        {
            // Обновляем локальный список
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (var column in Columns ?? new())
                {
                    var existing = column.Tasks.FirstOrDefault(t => t.Id == column.Id);
                    if (existing != null)
                    {
                        existing.Title = column.Title;
                    }
                }
            });
        }

        private async void OnColumnUpdatedFromServer(ColumnModel column)
        {
            // Обновляем локальный список
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
            // Обновляем локальный список
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Columns.Add(column);
            });
        }
        private async void OnColumnDeletedFromServer(int id)
        {
            // Обновляем локальный список
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var existing = Columns.FirstOrDefault(u => u.Id == id);
                if (existing != null)
                    Columns.Remove(existing);
            });
        }
        public string GetTimeLeft(TaskModel task)
        {
            if (!task.Deadline.HasValue)
                return "Без дедлайна";

            var deadline = task.Deadline.Value.Kind == DateTimeKind.Utc ? task.Deadline.Value.ToLocalTime() : task.Deadline.Value;
            var time = deadline - Now;

            if (time.TotalSeconds < 0)
                return "Просрочено";

            if (time.TotalDays >= 1)
                return $"{time.Days} д. {time.Hours} ч.";

            if (time.TotalHours >= 1)
                return $"{time.Hours} ч. {time.Minutes} мин.";

            if (time.TotalMinutes >= 1)
                return $"{time.Minutes} мин.";

            return "Меньше минуты";
        }

        // 24 часа до дедлайна
        public bool IsOverDeadline(TaskModel task)
        {
            if (!task.Deadline.HasValue)
                return false;

            return task.Deadline.Value.ToLocalTime() <= Now.AddHours(24);
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

        public EditColumnViewModel EditColumn { get; private set; }
        public bool IsEditColumnOpen
        {
            get => _isEditColumnOpen;
            set => this.RaiseAndSetIfChanged(ref _isEditColumnOpen, value);
        }
        private bool _isEditColumnOpen;
        
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

        private async Task OpenAddTaskDialogAsync(int columnId)
        {
            AddTask = new AddTaskViewModel(columnId, _navigationService, _userService, _labelService, _teamService);
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




        private void OpenEditTaskDialog(TaskModel task)
        {
            EditTask = new EditTaskViewModel(task, _userService, _labelService, _teamService);
            IsEditTaskOpen = true;

            EditTask.SaveCommand.Subscribe(async updatedTask =>
            {
                if (updatedTask == null)
                    return;

                try
                {
                    IsEditTaskOpen = false;
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

                var column = Columns.FirstOrDefault(c => c.Id == deleteTask.ColumnId);
                column?.Tasks.Remove(deleteTask);
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

        
    



        private async Task NavigateToSettingsAsync()
        {
            await _navigationService.NavigateToSettingsAsync();
        }
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

            // Удаляем из старой колонки
            oldColumn.Tasks.Remove(task);

            // Обновляем Position в старой колонке
            for (int i = 0; i < oldColumn.Tasks.Count; i++)
                oldColumn.Tasks[i].Position = i;

            // Добавляем в новую колонку
            task.ColumnId = newColumnId;
            task.Position = newPosition;
            newColumn.Tasks.Insert(newPosition, task);

            // Обновляем Position в новой колонке
            for (int i = 0; i < newColumn.Tasks.Count; i++)
                newColumn.Tasks[i].Position = i;

            // Сохраняем на сервере
            await _taskService.UpdateTaskAsync(task);
        }
        public async Task ReorderColumnAsync(int oldPosition, int newPosition)
        {
            var columns = Columns.OrderBy(c => c.Position).ToList();

            var column = columns[oldPosition];
            columns.RemoveAt(oldPosition);
            columns.Insert(newPosition, column);

            for (int i = 0; i < columns.Count; i++)
            {
                columns[i].Position = i;
                await _columnService.UpdateColumnAsync(columns[i]);
            }
        }
    }
}