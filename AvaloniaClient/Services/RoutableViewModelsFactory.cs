using AvaloniaClient.ViewModels;
using ReactiveUI;

namespace AvaloniaClient.Services;

public class RoutableViewModelsFactory(IScreen screen, IApiClient apiClient)
{
    private IScreen _screen = screen;
    private readonly IApiClient _apiClient = apiClient;

    //public CreateShapeViewModel CreateCreateUsersViewModel(NavigationService service) => new CreateUserViewModel(_screen, service, _userRepository);

    public ColumnViewModel CreateColumnViewModel(NavigationService service) => new ColumnViewModel(_screen, service, _apiClient);

}