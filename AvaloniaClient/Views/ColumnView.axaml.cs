using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaClient.ViewModels;

namespace AvaloniaClient;

public partial class ColumnView : ReactiveUserControl<ColumnViewModel>
{
    public ColumnView()
    {
        InitializeComponent();
    }
}