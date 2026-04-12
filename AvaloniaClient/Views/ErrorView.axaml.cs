using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaClient.ViewModels;

namespace AvaloniaClient;

public partial class ErrorView : ReactiveUserControl<ErrorViewModel>
{
    public ErrorView()
    {
        InitializeComponent();
    }
}