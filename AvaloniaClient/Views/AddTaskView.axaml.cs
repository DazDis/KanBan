using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaClient.ViewModels;
using ReactiveUI;

namespace AvaloniaClient;

public partial class AddTaskView : ReactiveUserControl<AddTaskViewModel>
{
    public AddTaskView()
    {
        InitializeComponent();
    }
}