using AvaloniaClient.DataBase;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive;

namespace AvaloniaClient.ViewModels
{
    public sealed class AddTaskViewModel : ViewModelBase
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _columnId;
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

        public string DeadlineInput
        {
            get => _deadlineInput;
            set
            {
                this.RaiseAndSetIfChanged(ref _deadlineInput, value);
                DataTimeParse();
            }
        }

        private void DataTimeParse()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(DeadlineInput))
                {
                    Deadline = DateTime.ParseExact(DeadlineInput, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                }
                else { Deadline = null; }
            }
            catch
            {
                Deadline = null;
            }
        }

        public ReactiveCommand<Unit, TaskModel?> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }


        public AddTaskViewModel(int columnId)
        {
            _columnId = columnId;

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
        }
    }
}
