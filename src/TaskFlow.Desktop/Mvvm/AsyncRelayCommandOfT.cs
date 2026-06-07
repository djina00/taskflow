namespace TaskFlow.Desktop.Mvvm;

/// <summary>
/// An <see cref="IRelayCommand"/> that runs an asynchronous handler taking a typed
/// command parameter — the bridge from a per-row button (passing its item via
/// <c>CommandParameter</c>) to an <c>async</c> use-case call. It disables itself while
/// running so a slow operation cannot be triggered twice.
/// </summary>
public sealed class AsyncRelayCommand<T> : IRelayCommand
{
    private readonly Func<T, Task> _execute;
    private readonly Func<T, bool>? _canExecute;
    private bool _isRunning;

    public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        !_isRunning && parameter is T value && (_canExecute?.Invoke(value) ?? true);

    public async void Execute(object? parameter)
    {
        if (parameter is not T value || !CanExecute(parameter))
            return;

        _isRunning = true;
        RaiseCanExecuteChanged();
        try
        {
            await _execute(value);
        }
        finally
        {
            _isRunning = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
