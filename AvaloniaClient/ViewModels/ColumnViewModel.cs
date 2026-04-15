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
        public ReactiveCommand<Unit, Unit> OpenAddColumnDialogCommand { get; }
        public ReactiveCommand<int, Unit> OpenAddTaskDialogCommand { get; }
        public ReactiveCommand<TaskModel, Unit> EditTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> DeleteTaskCommand { get; }


        public ColumnViewModel(IColumnService columnService, ITaskService taskService, IScreen screen)
        {
            _columnService = columnService;
            _taskService = taskService;
            HostScreen = screen;

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
                Columns[task.ColumnId-1].Tasks.Add(task);
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
    }
}