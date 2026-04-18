using AvaloniaClient.DataBase;
using ReactiveUI;
using System;
using System.Reactive;

namespace AvaloniaClient.ViewModels
{
	public class EditColumnViewModel : ReactiveObject
	{
	}
    public class EditColumnViewModel : ReactiveObject
    {
        private readonly ColumnModel _column;

        private string _title;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public ReactiveCommand<Unit, ColumnDTO?> SaveCommand { get; }
        public ReactiveCommand<Unit, int> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public EditColumnViewModel(ColumnModel column)
        {
            _column = column ?? throw new ArgumentNullException(nameof(column));

            Title = column.Title;

            SaveCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    return new ColumnDTO
                    {
                        Id = _column.Id,
                        Title = Title
                    };
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