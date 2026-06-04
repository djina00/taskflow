using System.Windows;
using System.Windows.Controls;
using TaskFlow.Desktop.ViewModels;

namespace TaskFlow.Desktop;

/// <summary>
/// The authentication gate shown before the main app. WPF's <see cref="PasswordBox"/>
/// deliberately does not expose its value through binding (so the password never
/// lives in a bindable property), so the code-behind forwards changes to the view
/// model — the only acceptable use of code-behind in this MVVM app.
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        PasswordInput.PasswordChanged += OnPasswordChanged;
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e) =>
        _viewModel.Password = ((PasswordBox)sender).Password;
}
