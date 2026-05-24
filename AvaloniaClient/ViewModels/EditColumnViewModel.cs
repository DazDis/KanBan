using AvaloniaClient.DataBase;
using ReactiveUI;
using System;
using System.Reactive;

namespace AvaloniaClient.ViewModels
{
	public class EditColumnViewModel : ViewModelBase
    {
        private readonly ColumnModel _column;

        private string _title;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        private string _error = string.Empty;

        public string Error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }
        private bool _haveError = false;
        public bool HaveError
        {
            get => _haveError;
            set => this.RaiseAndSetIfChanged(ref _haveError, value);
        }

        public ReactiveCommand<Unit, ColumnModel?> SaveCommand { get; }
        public ReactiveCommand<Unit, int> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public EditColumnViewModel(ColumnModel column)
        {
            _column = column ?? throw new ArgumentNullException(nameof(column));

            Title = column.Title;

            SaveCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Title))
                    {
                        Error = "Заполните название";
                        HaveError = true;
                        return null;
                    }
                    _column.Title = Title;
                    return _column;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            });

            DeleteCommand = ReactiveCommand.Create(() => _column.Id);

            CancelCommand = ReactiveCommand.Create(() => { });
        }
    }
}