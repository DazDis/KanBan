using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
    public sealed class ColumnViewModel : ViewModelBase, IRoutableViewModel
    {
        public string? UrlPathSegment => "board";
        public IScreen HostScreen { get; }
        private readonly IColumnService _columnService;
        private readonly ITaskService _taskService;
        public ObservableCollection<ColumnModel> Columns { get; } = new();
        private IReadOnlyList<TaskModel> Tasks = new List<TaskModel>();
        private bool IsInitialized;
        public ReactiveCommand<Unit, Task> NavigateToSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenAddColumnDialogCommand { get; }
        public ReactiveCommand<int, Unit> OpenAddTaskDialogCommand { get; }
        public ReactiveCommand<TaskModel, Unit> EditTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> DeleteTaskCommand { get; }

        public NavigationService _navigationService;
        public ColumnViewModel(IColumnService columnService, ITaskService taskService, NavigationService navigationService, IScreen screen)
        {
            _columnService = columnService;
            _taskService = taskService;
            _navigationService = navigationService;
            HostScreen = screen;

            NavigateToSettingsCommand = ReactiveCommand.Create(NavigateToSettingsAsync);
            OpenAddColumnDialogCommand = ReactiveCommand.Create(OpenAddColumnDialog);
            OpenAddTaskDialogCommand = ReactiveCommand.CreateFromTask<int>(OpenAddTaskDialogAsync);
        }


        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
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
                Columns[task.ColumnId].Tasks.Add(task);
            }
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

        private async Task CreateColumn(ColumnDTO dto)
        {
            try
            {
                IsAddColumnOpen = false;

                var created = await _columnService.CreateColumnAsync(dto);

                if (created != null)
                {
                    Columns.Add(new ColumnModel
                    {
                        Id = created.Id,
                        Title = created.Title,
                        Tasks = new ObservableCollection<TaskModel>()
                    });
                }
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

        private async Task OpenAddTaskDialogAsync(int columnId)
        {
            AddTask = new AddTaskViewModel(columnId);
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

        private async Task CreateTask(TaskModel task)
        {
            try
            {
                IsAddTaskOpen = false;

                var created = await _taskService.CreateTaskAsync(task);

                if (created != null)
                {
                    var column = Columns.FirstOrDefault(x => x.Id == created.ColumnId);
                    column?.Tasks.Add(created);
                }
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
            var tasks = column.Tasks.OrderBy(t => t.Position).ToList();

            var task = tasks[oldPosition];
            tasks.RemoveAt(oldPosition);
            tasks.Insert(newPosition, task);

            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Position = i;
                _taskService.UpdateTaskAsync(tasks[i]); 
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