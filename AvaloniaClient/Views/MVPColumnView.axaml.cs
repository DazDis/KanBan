using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaClient.ViewModels;

namespace AvaloniaClient;

public partial class MVPColumnView : ReactiveUserControl<ColumnViewModel>
{
    public MVPColumnView()
    {
        InitializeComponent();
    }
}