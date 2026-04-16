using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaClient.ViewModels;
using AvaloniaClient.ViewModels;
using System.Text.Json;

namespace AvaloniaClient;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();
    }
}