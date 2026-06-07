namespace TaskFlow.Desktop.Mvvm;

/// <summary>
/// A synchronous <see cref="IRelayCommand"/> that receives a typed command parameter —
/// used where a single command serves several buttons distinguished by their
/// <c>CommandParameter</c> (e.g. sidebar navigation passing the target section).
/// </summary>
public sealed class RelayCommand<T> : IRelayCommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter!) ?? true;

    public void Execute(object? parameter) => _execute((T)parameter!);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
