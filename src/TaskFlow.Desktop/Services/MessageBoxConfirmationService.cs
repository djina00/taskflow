using System.Windows;

namespace TaskFlow.Desktop.Services;

/// <summary>A confirmation prompt backed by the standard WPF message box.</summary>
public sealed class MessageBoxConfirmationService : IConfirmationService
{
    public bool Confirm(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
}
