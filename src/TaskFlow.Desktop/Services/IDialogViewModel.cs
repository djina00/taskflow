namespace TaskFlow.Desktop.Services;

/// <summary>
/// A view model that can be hosted in a modal dialog. It owns its own lifecycle: when
/// it is finished it raises <see cref="CloseRequested"/> with the result to return
/// (<c>true</c> when something changed, so the caller can refresh). Keeping this a
/// plain contract — no WPF types — leaves the dialog view models testable.
/// </summary>
public interface IDialogViewModel
{
    /// <summary>Raised when the dialog should close; the argument is the result to return.</summary>
    event Action<bool>? CloseRequested;

    /// <summary>The window title for the dialog.</summary>
    string DialogTitle { get; }
}
