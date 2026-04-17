// AvaloniaClient/Services/SignalRService.cs
using AvaloniaClient.DataBase;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace AvaloniaClient.Services;

public class SignalRService
{
    private HubConnection? _hubConnection;
    private readonly IConfigurationService _configService;

    public event Action<TaskModel>? TaskUpdated;
    public event Action<TaskModel>? TaskCreated;

    public SignalRService(IConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task StartAsync()
    {
        var baseUrl = _configService.GetApiUrl();
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}/taskHub")
            .Build();

        // Подписываемся на события от сервера
        _hubConnection.On<TaskModel>("TaskUpdated", task =>
        {
            TaskUpdated?.Invoke(task);
        });

        _hubConnection.On<TaskModel>("TaskCreated", task =>
        {
            TaskCreated?.Invoke(task);
        });

        await _hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    // Отправка обновления на сервер (если нужно)
    public async Task SendTaskUpdate(TaskModel task)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendTaskUpdate", task);
        }
    }
}