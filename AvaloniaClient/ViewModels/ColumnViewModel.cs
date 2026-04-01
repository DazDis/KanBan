using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{

    public sealed class ColumnViewModel : ViewModelBase, IRoutableViewModel
    {
        public bool IsInitialized { get; private set; }
        private IReadOnlyList<TaskModel> _tasks = new List<TaskModel>();
        private IScreen _screen;
        private NavigationService _navigationService;
        private IApiClient _apiClient;
        public IReadOnlyList<TaskModel> Tasks
        {
            get => _tasks;
            set => this.RaiseAndSetIfChanged(ref _tasks, value);
        }
        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get => _selectedTask;
            set => this.RaiseAndSetIfChanged(ref _selectedTask, value);
        }
        private ColumnModel _selectedColumn;
        public ColumnModel SelectedColumn
        {
            get => _selectedColumn;
            set => this.RaiseAndSetIfChanged(ref _selectedColumn, value);
        }
        public string? UrlPathSegment => "/users";

        public IScreen HostScreen { get; }
        public int a = 1;

        public ObservableCollection<ColumnModel> Columns { get; } = new();

        //public ReactiveCommand<int, Unit> AddTaskCommand { get; }
        public ReactiveCommand<int, Unit> OpenAddTaskDialogCommand { get; }
        public ReactiveCommand<Unit, Task> AddColumnCommand { get; }
        public ReactiveCommand<TaskModel, Unit> EditTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> DeleteTaskCommand { get; }

        private double _popupX;
        public double PopupX
        {
            get => _popupX;
            set => this.RaiseAndSetIfChanged(ref _popupX, value);
        }

        private double _popupY;
        public double PopupY
        {
            get => _popupY;
            set => this.RaiseAndSetIfChanged(ref _popupY, value);
        }

        private bool _isAddTaskPopupOpen;
        public bool IsAddTaskPopupOpen
        {
            get => _isAddTaskPopupOpen;
            set => this.RaiseAndSetIfChanged(ref _isAddTaskPopupOpen, value);
        }

        private AddTaskViewModel _addTaskViewModel;
        public AddTaskViewModel AddTaskViewModel
        {
            get => _addTaskViewModel;
            set => this.RaiseAndSetIfChanged(ref _addTaskViewModel, value);
        }


        public ColumnViewModel(IScreen screen, NavigationService navigationService, IApiClient apiClient) 
        { 
            _screen = screen;
            _navigationService = navigationService;
            _apiClient = apiClient;
            //var canDelete = this.WhenAnyValue(x => x.SelectedTask).Select(task => task != null);

            AddColumnCommand = ReactiveCommand.Create<Task>(AddColumnAsync);
            OpenAddTaskDialogCommand = ReactiveCommand.CreateFromTask<int>(OpenAddTaskDialogAsync);
            //EditTaskCommand = ReactiveCommand.Create<Task>(EditTaskAsync);
            //DeleteTaskCommand = ReactiveCommand.Create<TaskModel, Task>(DeleteTaskAsync, canDelete);
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
            Tasks = await _apiClient.GetAsync<List<TaskModel>>("api/task");

            /*Tasks = tasks?.Select(dto => new TaskModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                ColumnId = dto.ColumnId,
                LabelIds = dto.LabelIds,
                UserIds = dto.UserIds
            }).ToList() ?? new List<TaskModel>();*/
        }
        private async Task LoadColumns()
        {
            var columns = await _apiClient.GetAsync<List<ColumnDTO>>("api/column");
            foreach (var column in columns)
            {
                ColumnModel model = new ColumnModel()
                {
                    Id = column.Id,
                    Title = column.Title,
                    Tasks = new ObservableCollection<TaskModel>(),
                };

                Columns.Add(model);

            }
           /* Columns.Add(new ColumnModel { Id = 0, Title = "📋 To Do" });      // Id = 0
            Columns.Add(new ColumnModel { Id = 1, Title = "🔄 In Progress" }); // Id = 1
            Columns.Add(new ColumnModel { Id = 2, Title = "📝 Review" });      // Id = 2
            Columns.Add(new ColumnModel { Id = 3, Title = "✅ Done" });*/         // Id = 3
            foreach (var task in Tasks) {
                Columns[task.ColumnId].Tasks.Add(task);
            }
        }
        private async Task AddTaskAsync(TaskModel taskModel)
        {
            
            var createdTask = await _apiClient.PostAsync<TaskModel>("api/task", taskModel);
            if (createdTask != null)
            {
                var column = Columns.FirstOrDefault(c => c.Id == taskModel.ColumnId);
                column?.Tasks.Add(createdTask);
            }
        }
        private async Task AddColumnAsync()
        {
            ColumnDTO result = new ColumnDTO
            {
                //Id = 0,
                Title = "aaa",
            };
            var createdColumn = await _apiClient.PostAsync<ColumnDTO>("api/column", result);
            if (createdColumn != null)
            {
                ColumnModel model = new ColumnModel
                {
                    Id = result.Id,
                    Title = result.Title,
                    Tasks = new(),
                };
                Columns?.Add(model);
            }
        }
        private async Task OpenAddTaskDialogAsync(int columnId)
        {
            AddTaskViewModel = new AddTaskViewModel(columnId);
            IsAddTaskPopupOpen = true;
            PopupX = 200;
            PopupY = 150;
            // Подписываемся на результат
            AddTaskViewModel.SaveCommand.Subscribe(result =>
            {
                IsAddTaskPopupOpen = false;
                if (result != null)
                {
                    AddTaskAsync(result);
                }
            });

            AddTaskViewModel.CancelCommand.Subscribe(_ =>
            {
                IsAddTaskPopupOpen = false;
            });
        }

    }
}
