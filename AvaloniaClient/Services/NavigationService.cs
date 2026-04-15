using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace AvaloniaClient.Services;

public sealed class NavigationService
{
    private RoutableViewModelsFactory _routableViewModelsFactory;
    private IScreen _screen;

    public NavigationService(RoutableViewModelsFactory routableViewModelsFactory, IScreen screen)
    {
        _routableViewModelsFactory = routableViewModelsFactory;
        _screen = screen;
    }

    public async Task NavigateToColumnAsync()
    {
        var columnViewModel = _routableViewModelsFactory.CreateColumnViewModel(this, _screen);
        await columnViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(columnViewModel);
    }
    public async Task NavigateToErrorAsync()
    {
        var errorViewModel = _routableViewModelsFactory.CreateErrorViewModel(this, _screen);
        await errorViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(errorViewModel);
    }
    public async Task NavigateToSettingsAsync()
    {
        var settingsViewModel = _routableViewModelsFactory.CreateSettingsViewModel(this, _screen);
        await settingsViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(settingsViewModel);
    }

}
