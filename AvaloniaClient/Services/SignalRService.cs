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
    public event Action<ColumnModel>? ColumnUpdated;
    public event Action<ColumnModel>? ColumnCreated;
    public event Action<LabelModel>? LabelUpdated;
    public event Action<LabelModel>? LabelCreated;
    public event Action<UserModel>? UserUpdated;
    public event Action<UserModel>? UserCreated;
    public event Action<TeamModel>? TeamUpdated;
    public event Action<TeamModel>? TeamCreated;
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

        _hubConnection.On<ColumnModel>("ColumnCreated", column =>
        {
            ColumnCreated?.Invoke(column);
        });

        _hubConnection.On<ColumnModel>("ColumnUpdated", column =>
        {
            ColumnUpdated?.Invoke(column);
        });

        _hubConnection.On<LabelModel>("LabelCreated", label =>
        {
            LabelCreated?.Invoke(label);
        });

        _hubConnection.On<LabelModel>("LabelUpdated", label =>
        {
            LabelUpdated?.Invoke(label);
        });

        _hubConnection.On<UserModel>("UserCreated", user =>
        {
            UserCreated?.Invoke(user);
        });

        _hubConnection.On<UserModel>("UserUpdated", user =>
        {
            UserUpdated?.Invoke(user);
        });

        _hubConnection.On<TeamModel>("TeamCreated", team =>
        {
            TeamCreated?.Invoke(team);
        });

        _hubConnection.On<TeamModel>("TeamUpdated", team =>
        {
            TeamUpdated?.Invoke(team);
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