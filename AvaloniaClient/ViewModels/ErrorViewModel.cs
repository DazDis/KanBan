using AvaloniaClient.DataBase;
using AvaloniaClient.Services;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaloniaClient.ViewModels
{
    public sealed class ErrorViewModel : ViewModelBase, IRoutableViewModel
    {
       
        private bool _isServerAvailable = true;
        public bool IsServerAvailable
        {
            get => _isServerAvailable;
            set => this.RaiseAndSetIfChanged(ref _isServerAvailable, value);
        }
        private string _statusMessage = "Загрузка...";


        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }
        private string _serverUrl;
        public string ServerUrl
        {
            get => _serverUrl;
            set
            {
                this.RaiseAndSetIfChanged(ref _serverUrl, value);
            }
        }
        private readonly NavigationService _navigationService;
        private readonly IHealthService _healthService;
        public ReactiveCommand<Unit, Task> RetryCommand { get; }
        private bool IsInitialized;
        private IConfigurationService _configService;

        public string? UrlPathSegment => "error";
        public IScreen HostScreen { get; }

        public ErrorViewModel(NavigationService navigationService, IHealthService healthService, IConfigurationService configuration)
        {
            _navigationService = navigationService;
            _healthService = healthService;
            _configService = configuration;
            RetryCommand = ReactiveCommand.Create(RetryConnect);
        }
        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                LoadInitialView();
            }
        }
        private async void LoadInitialView()
        {
            var health = await _healthService.GetHealthStatusAsync();

            if (health.IsAvailable)
            {
                IsServerAvailable = true;
                await _navigationService.NavigateToColumnAsync();
            }
            else
            {
                IsServerAvailable = false;
                StatusMessage = health.Message;
                ServerUrl = _configService.GetApiUrl();
            }
        }

        private async Task RetryConnect()
        {
            _configService.SaveApiUrl(ServerUrl);
            _healthService.ResetUrl();
            var health = await _healthService.GetHealthStatusAsync();

            if (health.IsAvailable)
            {
                IsServerAvailable = true;
                await _navigationService.NavigateToColumnAsync();
            }
            else
            {
                StatusMessage = health.Message;
            }
        }
    }
}
