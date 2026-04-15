using AvaloniaClient.ViewModels;
using ReactiveUI;

namespace AvaloniaClient.Services;

public class RoutableViewModelsFactory(IColumnService columnService, ITaskService taskService, ILabelService labelService, IUserService userService, ITeamService teamService, IHealthService healthService, IConfigurationService configurationService)
{
    //private IScreen _screen = screen;
    //private readonly IApiClient _apiClient = apiClient;
    private readonly IColumnService _columnService = columnService;
    private readonly ITaskService _taskService = taskService;
    private readonly ITeamService _teamService = teamService;
    private readonly IUserService _userService = userService;
    private readonly ILabelService _labelService = labelService;
    private readonly IHealthService _healthService = healthService;
    private readonly IConfigurationService _configuration = configurationService;

    //public CreateShapeViewModel CreateCreateUsersViewModel(NavigationService service) => new CreateUserViewModel(_screen, service, _userRepository);

    public ColumnViewModel CreateColumnViewModel(NavigationService navigationService, IScreen screen) => new ColumnViewModel(_columnService, _taskService, navigationService, screen);
    public ErrorViewModel CreateErrorViewModel(NavigationService navigationService, IScreen screen) => new ErrorViewModel(navigationService, _healthService, _configuration, screen);
    public SettingsViewModel CreateSettingsViewModel(NavigationService navigationService, IScreen screen) => new SettingsViewModel(_configuration, _userService, _labelService, screen, navigationService);

}