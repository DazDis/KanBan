using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using AvaloniaClient.DataBase;
using AvaloniaClient.ViewModels;
using System.Linq;

namespace AvaloniaClient.Services
{
    public class TaskDropHandler : DropHandlerBase
    {
        // Проверка: можно ли выполнить перенос
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.Data.Contains("Context");
        }

        // Логика выполнения переноса
        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            var draggedTask = e.Data.Get("Context") as TaskModel;
            if (draggedTask == null) return false;
            // Находим ColumnView и его ViewModel
            ColumnViewModel viewModel;
            Control control = sender as Control;
            viewModel = (ColumnViewModel)(control.FindLogicalAncestorOfType<ColumnView>()?.DataContext);
            ColumnModel targetColumn = null;
            int targetIndex = -1;

            // Определяем целевую колонку и индекс
            if (targetContext is TaskModel targetTask)
            {
                // Нашли задачу, значит бросаем на неё
                targetColumn = viewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(targetTask));
                if (targetColumn != null)
                {
                    targetIndex = targetColumn.Tasks.IndexOf(targetTask);
                    // Если бросаем на ту же задачу, ничего не делаем
                    if (draggedTask.Id == targetTask.Id) return false;
                }
            }
            else if (targetContext is ColumnModel column)
            {
                // Бросаем на колонку (пустое место)
                targetColumn = column;
                targetIndex = targetColumn.Tasks.Count - 1; // в конец
            }
            else if (sender is ItemsControl itemsControl && itemsControl.DataContext is ColumnModel columnFromSender)
            {
                // Альтернативный способ: если targetContext не дал колонку, берём из sender
                targetColumn = columnFromSender;
                targetIndex = targetColumn.Tasks.Count;
            }

            if (targetColumn == null || targetIndex < 0) return false;

            // Определяем старую колонку и позицию
            var oldColumn = viewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(draggedTask));
            if (oldColumn == null) return false;
            int oldPosition = oldColumn.Tasks.IndexOf(draggedTask);

            if (oldColumn.Id == targetColumn.Id)
            {
                // Перемещение внутри одной колонки
                // Корректируем позицию, если перемещаем вниз
                int newPos = targetIndex;
                viewModel.ReorderTaskInColumn(targetColumn.Id, oldPosition, newPos);
            }
            else
            {
                // Перемещение между колонками
                _ = viewModel.MoveTaskToColumnAsync(draggedTask, targetColumn.Id, targetIndex);
            }

            return true;
        }

    }
}
