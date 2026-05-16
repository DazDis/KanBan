using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using AvaloniaClient.DataBase;

namespace AvaloniaClient.Services
{
    public class TeamDropHandler : DropHandlerBase
    {
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.Data.Contains("DragData");
        }

        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            var draggedUser = e.Data.Get("DragData") as UserModel;
            var targetTeam = targetContext as TeamModel;
            if (draggedUser != null && targetTeam != null)
            {
                System.Diagnostics.Debug.WriteLine($"Перетащен пользователь {draggedUser.FirstName} в команду {targetTeam.Title}");
                // Здесь будет логика добавления пользователя в команду
                return true;
            }
            return false;
        }
    }
}