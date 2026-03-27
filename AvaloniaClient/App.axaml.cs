using Avalonia;
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

namespace AvaloniaClient
{
    public partial class App : Application
    {
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                RegisterLocatorComponents();
                _serviceProvider = CreateContainer();

                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
            var navigationService = _serviceProvider.GetRequiredService<NavigationService>();
            _ = navigationService.NavigateToColumnAsync();
        }

        private IServiceProvider CreateContainer()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiBaseUrl"]);
            });
            services.AddScoped<NavigationService>();
            services.AddScoped<RoutableViewModelsFactory>();
            services.AddScoped<MainWindowViewModel>();
            services.AddTransient<ColumnViewModel>();
            services.AddScoped<IScreen>(provider => provider.GetRequiredService<MainWindowViewModel>());
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
            Locator.CurrentMutable.Register<IViewFor<ColumnViewModel>>(() => new MVPColumnView());
        }
    }
}