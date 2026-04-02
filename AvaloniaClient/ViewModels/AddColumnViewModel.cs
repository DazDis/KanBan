using AvaloniaClient.DataBase;
using ReactiveUI;
using System;
using System.Reactive;

namespace AvaloniaClient.ViewModels
{
    public sealed class AddColumnViewModel : ViewModelBase
    {
        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public ReactiveCommand<Unit, ColumnDTO?> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddColumnViewModel()
        {
            SaveCommand = ReactiveCommand.Create(() =>
            {
                try
            {
                return new ColumnDTO
                {
                    Title = Title
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