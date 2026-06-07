using System.Linq;
using System.Windows;

namespace TaskFlow.Desktop.Services;

/// <summary>
/// Shows a dialog view model in a <see cref="DialogWindow"/>. The window is supplied
/// by a factory (so a fresh, DI-built one is used each time), owned by the active
/// window for correct modality and centering, and closed when the view model raises
/// <see cref="IDialogViewModel.CloseRequested"/>.
/// </summary>
public sealed class DialogService : IDialogService
{
    private readonly Func<DialogWindow> _windowFactory;

    public DialogService(Func<DialogWindow> windowFactory) => _windowFactory = windowFactory;

    public bool ShowDialog(IDialogViewModel viewModel)
    {
        var window = _windowFactory();
        window.DataContext = viewModel;
        window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            ?? Application.Current.MainWindow;

        void OnClose(bool result) => window.DialogResult = result;   // setting DialogResult closes the modal window
        viewModel.CloseRequested += OnClose;
        try
        {
            return window.ShowDialog() == true;
        }
        finally
        {
            viewModel.CloseRequested -= OnClose;
        }
    }
}
