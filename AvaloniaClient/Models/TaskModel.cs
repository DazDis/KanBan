using ReactiveUI;
using System;
using System.Collections.Generic;

namespace AvaloniaClient.DataBase
{
    public class TaskModel : ReactiveObject
    {
        private int _id;
        private string _title;
        private string _description;
        private DateTime? _deadline;
        private int _columnId;
        private string? _color;
        private List<int?> _userIds = new();
        private List<int?> _teamIds = new();
        private List<int?> _labelIds = new();
        private int _position;
        private string? _timeLeft;
        private bool? _overDeadline;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

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

        public int ColumnId
        {
            get => _columnId;
            set => this.RaiseAndSetIfChanged(ref _columnId, value);
        }

        public string? Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public List<int?> UserIds
        {
            get => _userIds;
            set => this.RaiseAndSetIfChanged(ref _userIds, value);
        }

        public List<int?> TeamIds
        {
            get => _teamIds;
            set => this.RaiseAndSetIfChanged(ref _teamIds, value);
        }

        public List<int?> LabelIds
        {
            get => _labelIds;
            set => this.RaiseAndSetIfChanged(ref _labelIds, value);
        }

        public int Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }

        public string? TimeLeft
        {
            get => _timeLeft;
            set => this.RaiseAndSetIfChanged(ref _timeLeft, value);
        }

        public bool? OverDeadline
        {
            get => _overDeadline;
            set => this.RaiseAndSetIfChanged(ref _overDeadline, value);
        }
    }
}