namespace TaskFlow.Desktop.Services;

/// <summary>
/// Asks the user to confirm a destructive action. Abstracting the prompt behind an
/// interface keeps the view models free of any UI-framework dependency, so they stay
/// testable and the dialog technology can change in one place.
/// </summary>
public interface IConfirmationService
{
    /// <summary>Returns <c>true</c> only if the user explicitly confirms.</summary>
    bool Confirm(string title, string message);
}
