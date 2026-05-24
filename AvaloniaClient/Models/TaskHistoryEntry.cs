using ReactiveUI;
using System;

namespace AvaloniaClient.Models
{
    public class TaskHistoryEntry : ReactiveObject
    {
        private int _id;
        private int _taskId;
        private string _actionType; // "Created", "ColumnChanged", "DescriptionChanged", "DeadlineChanged", "AssigneeChanged"
        private string _oldValue;
        private string _newValue;
        private DateTime _changedAt;
        private string _comment;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public int TaskId
        {
            get => _taskId;
            set => this.RaiseAndSetIfChanged(ref _taskId, value);
        }

        public string ActionType
        {
            get => _actionType;
            set => this.RaiseAndSetIfChanged(ref _actionType, value);
        }

        public string OldValue
        {
            get => _oldValue;
            set => this.RaiseAndSetIfChanged(ref _oldValue, value);
        }

        public string NewValue
        {
            get => _newValue;
            set => this.RaiseAndSetIfChanged(ref _newValue, value);
        }

        public DateTime ChangedAt
        {
            get => _changedAt;
            set => this.RaiseAndSetIfChanged(ref _changedAt, value);
        }

        public string Comment
        {
            get => _comment;
            set => this.RaiseAndSetIfChanged(ref _comment, value);
        }

        public string DisplayText => $"{ChangedAt:HH:mm:ss dd.MM.yyyy} - {ActionType}: {OldValue} → {NewValue}";
    }
}