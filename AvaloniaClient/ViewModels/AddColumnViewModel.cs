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
        public ReactiveCommand<Unit, ColumnDTO?> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddColumnViewModel()
        {
            SaveCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Title))
                    {
                        Error = "Заполните название";
                        HaveError = true;
                        return null;
                    }

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