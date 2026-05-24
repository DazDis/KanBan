using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using AvaloniaClient.DataBase;
using AvaloniaClient.ViewModels;
using System.Linq;

namespace AvaloniaClient.Services;

public class UnifiedDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        // Проверяем, есть ли данные для задачи или колонки
        return e.Data.Contains("Context");
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        var data = e.Data.Get("Context");

        // Пробуем обработать как задачу
        if (data is TaskModel draggedTask)
        {
            return HandleTaskDrop(sender, e, targetContext, draggedTask);
        }

        // Пробуем обработать как колонку
        if (data is ColumnModel draggedColumn)
        {
            return HandleColumnDrop(sender, e, targetContext, draggedColumn);
        }

        return false;
    }

    private bool HandleTaskDrop(object? sender, DragEventArgs e, object? targetContext, TaskModel draggedTask)
    {
        // Находим ViewModel
        if (sender is not Control control) return false;
        var viewModel = control.FindLogicalAncestorOfType<ColumnView>()?.DataContext as ColumnViewModel;
        if (viewModel == null) return false;

        ColumnModel targetColumn = null;
        int targetIndex = -1;

        // Определяем целевую колонку и индекс
        if (targetContext is TaskModel targetTask)
        {
            targetColumn = viewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(targetTask));
            if (targetColumn != null)
            {
                targetIndex = targetColumn.Tasks.IndexOf(targetTask);
                if (draggedTask.Id == targetTask.Id) return false;
            }
        }
        else if (targetContext is ColumnModel column)
        {
            targetColumn = column;
            targetIndex = targetColumn.Tasks.Count - 1; 
        }
        else if (sender is ItemsControl itemsControl && itemsControl.DataContext is ColumnModel columnFromSender)
        {
            targetColumn = columnFromSender;
            targetIndex = targetColumn.Tasks.Count;
        }

        var oldColumn = viewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(draggedTask));
        if (oldColumn == null) return false;
        int oldPosition = oldColumn.Tasks.IndexOf(draggedTask);
        if (oldColumn.Id != targetColumn.Id)
            targetIndex += 1;

        if (targetColumn == null || targetIndex < 0) return false;

        if (oldColumn.Id == targetColumn.Id)
        {
            viewModel.ReorderTaskInColumn(targetColumn.Id, oldPosition, targetIndex);
        }
        else
        {
            _ = viewModel.MoveTaskToColumnAsync(draggedTask, targetColumn.Id, targetIndex);
        }
        return true;
    }

    private bool HandleColumnDrop(object? sender, DragEventArgs e, object? targetContext, ColumnModel draggedColumn)
    {
        if (sender is not Control control) return false;
        var viewModel = control.FindLogicalAncestorOfType<ColumnView>()?.DataContext as ColumnViewModel;
        if (viewModel == null) return false;

        if (targetContext is ColumnModel targetColumn)
        {
            _ = viewModel.ReorderColumnAsync(draggedColumn, targetColumn);
            return true;
        }

        if (sender is ItemsControl && targetContext == null)
        {
            _ = viewModel.ReorderColumnAsync(draggedColumn, viewModel.Columns.Count - 1);
            return true;
        }
        return false;
    }
}