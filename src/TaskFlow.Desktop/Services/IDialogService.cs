namespace TaskFlow.Desktop.Services;

/// <summary>
/// Shows a view model in a modal dialog and reports whether it closed with changes.
/// Abstracted so view models can open a dialog without referencing any WPF window
/// type, keeping the presentation logic testable and the window technology swappable.
/// </summary>
public interface IDialogService
{
    /// <summary>Shows <paramref name="viewModel"/> modally; returns <c>true</c> if it closed with changes.</summary>
    bool ShowDialog(IDialogViewModel viewModel);
}
