using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using AvaloniaClient.DataBase;
using AvaloniaClient.ViewModels;

namespace AvaloniaClient.Services;

public class ColumnDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return e.Data.Contains("ColumnDragData");
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        var draggedColumn = e.Data.Get("ColumnDragData") as ColumnModel;
        if (draggedColumn == null) return false;

        if (targetContext is ColumnModel targetColumn)
        {
            if (sender is Control control && control.FindLogicalAncestorOfType<ColumnView>()?.DataContext is ColumnViewModel viewModel)
            {
                _ = viewModel.ReorderColumnAsync(draggedColumn, targetColumn);
                return true;
            }
        }
        if (sender is ItemsControl itemsControl && targetContext == null)
        {
            var viewModel = itemsControl.FindLogicalAncestorOfType<ColumnView>()?.DataContext as ColumnViewModel;
            if (viewModel != null)
            {
                _ = viewModel.ReorderColumnAsync(draggedColumn, viewModel.Columns.Count - 1);
                return true;
            }
        }
        return false;
    }
}