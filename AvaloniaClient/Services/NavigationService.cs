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
        var columnViewModel = _routableViewModelsFactory.CreateColumnViewModel(this);
        await columnViewModel.InitializeAsync();
        await _screen.Router.Navigate.Execute(columnViewModel);
    }
}
