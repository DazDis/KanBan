// Server/Hubs/TaskHub.cs
using Microsoft.AspNetCore.SignalR;
using Server.DataBase;
using Server.DTOs;

namespace Server.Hubs;

public class TaskHub : Hub
{
    // Метод, который вызывает клиент
    public async Task SendTaskUpdate(TaskDTO task)
    {
        // Рассылаем всем подключенным клиентам
        await Clients.All.SendAsync("TaskUpdated", task);
    }

    public async Task SendColumnUpdate(ColumnDTO column)
    {
        await Clients.All.SendAsync("ColumnUpdated", column);
    }

    // Когда клиент подключается
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", $"Connected with ID: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }
}