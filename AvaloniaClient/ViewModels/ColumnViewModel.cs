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
        public string? UrlPathSegment => "/users";

        public IScreen HostScreen { get; }


        public ObservableCollection<ColumnModel> Columns { get; } = new();

        public ReactiveCommand<int, Unit> AddTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> EditTaskCommand { get; }
        public ReactiveCommand<TaskModel, Unit> DeleteTaskCommand { get; }
        public ColumnViewModel(IScreen screen, NavigationService navigationService, IApiClient apiClient) 
        { 
            _screen = screen;
            _navigationService = navigationService;
            _apiClient = apiClient;
            //var canDelete = this.WhenAnyValue(x => x.SelectedTask).Select(task => task != null);

            AddTaskCommand = ReactiveCommand.CreateFromTask<int>(AddTaskAsync);
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
            Columns.Add(new ColumnModel { Id = 0, Title = "📋 To Do" });      // Id = 0
            Columns.Add(new ColumnModel { Id = 1, Title = "🔄 In Progress" }); // Id = 1
            Columns.Add(new ColumnModel { Id = 2, Title = "📝 Review" });      // Id = 2
            Columns.Add(new ColumnModel { Id = 3, Title = "✅ Done" });         // Id = 3
            foreach (var task in Tasks) {
                Columns[task.ColumnId].Tasks.Add(task);
            }
        }
        private async Task AddTaskAsync(int columnId)
        {
            TaskModel result = new TaskModel
            {
                Id = 0,
                Title = "aaa",
                Description = "bbb",
                ColumnId = columnId,
                UserIds = [0],
                LabelIds = [0],
            };
            var createdTask = await _apiClient.PostAsync<TaskModel>("api/task", result);
            if (createdTask != null)
            {
                var column = Columns.FirstOrDefault(c => c.Id == columnId);
                column?.Tasks.Add(createdTask);
            }
        }



    }
}
