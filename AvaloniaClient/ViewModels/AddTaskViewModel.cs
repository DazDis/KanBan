using Avalonia.Media;
using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;



namespace AvaloniaClient.ViewModels
{
    public sealed class AddTaskViewModel : ViewModelBase
    {
        public NavigationService _navigationService;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _columnId;
        private DateTimeOffset? _date;
        private TimeSpan? _time;
        private DateTime? _deadline;
        private string _deadlineInput = string.Empty;

        

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

        public AddTaskViewModel(int columnId, NavigationService navigationService)
        {
            _columnId = columnId;
            _navigationService = navigationService;

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    return (TaskModel?)new TaskModel
                    {
                        Title = Title,
                        Description = Description,
                        Deadline = Deadline,
                        ColumnId = _columnId,
                        Color = SelectedColor.ToString(),
                        UserIds = new List<int?>(),
                        LabelIds = new List<int?>(),
                        TeamIds = new List<int?>(),
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
    }
}