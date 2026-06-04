using System.Windows;
using TaskFlow.Desktop.ViewModels;

namespace TaskFlow.Desktop;

/// <summary>
/// The application shell. Its data context is the <see cref="MainViewModel"/>,
/// injected from the composition root; each tab binds to one of its feature view
/// models.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
