using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using AvaloniaClient.DataBase;
using AvaloniaClient.ViewModels;
using System.Linq;

namespace AvaloniaClient.Services;

public class TeamDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return e.Data.Contains("Context");
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        var draggedUser = e.Data.Get("Context") as UserModel;
        if (draggedUser == null) return false;

        if (targetContext is TeamModel targetTeam)
        {
            if (sender is Control control && control.FindLogicalAncestorOfType<SettingsView>()?.DataContext is SettingsViewModel vm)
            {
                vm.AddUserToTeam(draggedUser, targetTeam);
                return true;
            }
        }
        if (targetContext is SettingsViewModel || sender is ItemsControl)
        {
            if (sender is Control control && control.FindLogicalAncestorOfType<SettingsView>()?.DataContext is SettingsViewModel vm)
            {
                var teamsWithUser = vm.Teams.Where(t => t.Users.Contains(draggedUser));
                foreach(var team in teamsWithUser)
                {
                    _ = vm.RemoveUserFromTeam(draggedUser, team);
                }
                return true;
            }
        }
        return false;
    }
}