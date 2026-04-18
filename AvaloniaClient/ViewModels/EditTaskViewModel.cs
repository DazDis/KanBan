using AvaloniaClient.DataBase;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace AvaloniaClient.ViewModels
{
	public class EditTaskViewModel : ViewModelBase

    {
        
        private readonly TaskModel _task;

        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _columnId;
        private string _selectedColor = string.Empty;
        private DateTime? _date;
        private TimeSpan? _time;



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

        public EditTaskViewModel(TaskModel task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));

            _title = _task.Title;
            _description = _task.Description;
            _columnId = _task.ColumnId;

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    _task.Title = Title;
                    _task.Description = Description;
                    _task.ColumnId = _columnId;

                    return _task;
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