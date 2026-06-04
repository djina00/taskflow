using System.Windows;
using System.Windows.Controls;
using TaskFlow.Desktop.ViewModels;

namespace TaskFlow.Desktop;

/// <summary>
/// The account-creation screen, reachable from the login window. As with
/// <see cref="LoginWindow"/>, WPF's <see cref="PasswordBox"/> deliberately does not
/// expose its value through binding, so the code-behind forwards changes to the view
/// model — the only acceptable use of code-behind in this MVVM app.
/// </summary>
public partial class RegisterWindow : Window
{
    private readonly RegisterViewModel _viewModel;

    public RegisterWindow(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        PasswordInput.PasswordChanged += OnPasswordChanged;
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e) =>
        _viewModel.Password = ((PasswordBox)sender).Password;
}
