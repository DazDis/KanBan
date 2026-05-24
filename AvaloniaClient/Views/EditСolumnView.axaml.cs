using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaClient.ViewModels;

namespace AvaloniaClient;

public partial class EditСolumnView : ReactiveUserControl<EditColumnViewModel>
{
    public EditСolumnView()
    {
        InitializeComponent();
    }
}