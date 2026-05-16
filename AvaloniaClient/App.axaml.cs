using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaClient.Services;
using AvaloniaClient.ViewModels;
using AvaloniaClient.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactiveUI;
using Splat;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using static System.Net.WebRequestMethods;

namespace AvaloniaClient
{
    public partial class App : Application
    {
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        public static Window? MainWindow { get; private set; }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() { 

        //if (!Design.IsDesignMode)
            
        //{
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                RegisterLocatorComponents();
                _serviceProvider = CreateContainer();

                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                try
                {
                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = _serviceProvider?.GetRequiredService<MainWindowViewModel>(),
                    };
                    MainWindow = desktop.MainWindow;
                }
                catch
                {
                    throw new NotImplementedException();
                }
            }

            
            base.OnFrameworkInitializationCompleted();
            try
            {
                var navigationService = _serviceProvider?.GetRequiredService<NavigationService>();
                _ = navigationService?.NavigateToErrorAsync();
            }
            catch
            {
                throw new NotImplementedException();
            }

            }
        //}
        private IServiceProvider CreateContainer()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IApiClient>(provider =>
            {
                var configService = provider.GetRequiredService<IConfigurationService>();
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(configService.GetApiUrl()),
                    Timeout = TimeSpan.FromSeconds(30)
                };
                return new ApiClient(httpClient);
            });
            services.AddScoped<NavigationService>();
            services.AddScoped<RoutableViewModelsFactory>();
            services.AddScoped<MainWindowViewModel>();
            services.AddTransient<ColumnViewModel>();
            services.AddTransient<ErrorViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddSingleton<SignalRService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IHealthService, HealthService>();
            services.AddScoped<IColumnService, ColumnService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILabelService, LabelService>();
            services.AddScoped<ITaskService, TaskService>();
            try
            {
                services.AddScoped<IScreen>(provider => provider?.GetRequiredService<MainWindowViewModel>());
            }
            catch
            {
                throw new NotImplementedException();
            }
            return services.BuildServiceProvider();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
        private static void RegisterLocatorComponents()
        {
            Locator.CurrentMutable.Register<IViewFor<ColumnViewModel>>(() => new ColumnView());
            Locator.CurrentMutable.Register<IViewFor<ErrorViewModel>>(() => new ErrorView());
            Locator.CurrentMutable.Register<IViewFor<SettingsViewModel>>(() => new SettingsView());
        }
    }
}