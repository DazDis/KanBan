using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System;

namespace AvaloniaClient.DataBase
{
    public class TaskModel : ReactiveObject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
        public List<int?> UserIds { get; set; }
        public List<int?> TeamIds { get; set; }
        public List<int?> LabelIds { get; set; }
        public string? Color { get; set; }
        private string _timeLeft;
        public string TimeLeft
        {
            get => _timeLeft;
            set => this.RaiseAndSetIfChanged(ref _timeLeft, value);
        }
        private bool _overDeadline;
        public bool OverDeadline
        {
            get => _overDeadline;
            set => this.RaiseAndSetIfChanged(ref _overDeadline, value);
        }
        
        public int Position { get; set; }

    }
}