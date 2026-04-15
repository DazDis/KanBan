using AvaloniaClient.ViewModels;
using ReactiveUI;

namespace AvaloniaClient.Services;

public class RoutableViewModelsFactory(IColumnService columnService, ITaskService taskService, IHealthService healthService, IConfigurationService configurationService, NavigationService navigationService)
{
    //private IScreen _screen = screen;
    //private readonly IApiClient _apiClient = apiClient;
    private readonly NavigationService _navigationService = navigationService;
    private readonly IColumnService _columnService = columnService;
    private readonly ITaskService _taskService = taskService;
    private readonly IHealthService _healthService = healthService;
    private readonly IConfigurationService _configuration = configurationService;

    //public CreateShapeViewModel CreateCreateUsersViewModel(NavigationService service) => new CreateUserViewModel(_screen, service, _userRepository);

    public ColumnViewModel CreateColumnViewModel(IScreen screen) => new ColumnViewModel(_columnService, _taskService, _navigationService, screen);
    public ErrorViewModel CreateErrorViewModel(IScreen screen) => new ErrorViewModel(_navigationService, _healthService, _configuration, screen);
    public ErrorViewModel CreateSettingsViewModel(IScreen screen) => new ErrorViewModel(_navigationService, _healthService, _configuration, screen);

}