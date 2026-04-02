using AvaloniaClient.ViewModels;
using ReactiveUI;

namespace AvaloniaClient.Services;

public class RoutableViewModelsFactory(IColumnService columnService, ITaskService taskService)
{
    //private IScreen _screen = screen;
    //private readonly IApiClient _apiClient = apiClient;
    private readonly IColumnService _columnService = columnService;
    private readonly ITaskService _taskService = taskService;

    //public CreateShapeViewModel CreateCreateUsersViewModel(NavigationService service) => new CreateUserViewModel(_screen, service, _userRepository);

    public ColumnViewModel CreateColumnViewModel(IScreen screen) => new ColumnViewModel(_columnService, _taskService, screen);

}