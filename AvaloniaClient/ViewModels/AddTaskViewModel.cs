using AvaloniaClient.DataBase;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace AvaloniaClient.ViewModels
{
    public sealed class AddTaskViewModel : ViewModelBase
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _columnId;

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
                        ColumnId = _columnId,
                        UserIds = new List<int?>(),
                        LabelIds = new List<int?>()
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
