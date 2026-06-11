using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AvaloniaClient.Services;

public class NavigationService
{
    private RoutableViewModelsFactory _routableViewModelsFactory;
    private readonly IServiceProvider _serviceProvider;
    private IScreen _screen;

    public NavigationService(RoutableViewModelsFactory routableViewModelsFactory, IScreen screen)
    {
        _routableViewModelsFactory = routableViewModelsFactory;
        _screen = screen;
    }

    public async Task NavigateToColumnAsync()
    {
        var columnViewModel = _routableViewModelsFactory.CreateColumnViewModel();
        await columnViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(columnViewModel);
    }
    public async Task NavigateToErrorAsync()
    {
        var errorViewModel = _routableViewModelsFactory.CreateErrorViewModel();
        await errorViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(errorViewModel);
    }
    public async Task NavigateToSettingsAsync()
    {
        var settingsViewModel = _routableViewModelsFactory.CreateSettingsViewModel();
        await settingsViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(settingsViewModel);
    }

}
