using AvaloniaClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvaloniaClient.Services;

public class RoutableViewModelsFactory(IServiceProvider serviceProvider)
{
    private IServiceProvider _serviceProvider = serviceProvider;

    public ColumnViewModel CreateColumnViewModel() => _serviceProvider.GetRequiredService<ColumnViewModel>();
    public ErrorViewModel CreateErrorViewModel() => _serviceProvider.GetRequiredService<ErrorViewModel>();
    public SettingsViewModel CreateSettingsViewModel() => _serviceProvider.GetRequiredService<SettingsViewModel>();

}