using TaskFlow.Desktop.Mvvm;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Base for the feature view models behind each section of the shell. It carries the
/// concerns every feature repeats — the shared session, a user-facing status line, a
/// busy flag, command re-evaluation on session changes, and a uniform way to run a
/// use case and surface its <see cref="Result"/> — so the concrete view models stay
/// focused on their own commands and queries.
/// </summary>
public abstract class FeatureViewModelBase : ViewModelBase
{
    private string _status = string.Empty;
    private bool _isBusy;

    protected FeatureViewModelBase(SessionContext session) => Session = session;

    /// <summary>The shared UI session, exposed so views can bind to <c>Session.*</c> auth and selection state.</summary>
    public SessionContext Session { get; }

    /// <summary>The latest outcome message — a validation/domain error or a short success note.</summary>
    public string Status
    {
        get => _status;
        protected set => SetProperty(ref _status, value);
    }

    /// <summary>True while an asynchronous command is running, for an optional busy affordance in the view.</summary>
    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// Called when this feature becomes the active section, so it can load its data
    /// without the user pressing Refresh. The default does nothing; features that show
    /// data override it. The shell invokes it best-effort, without awaiting.
    /// </summary>
    public virtual Task OnActivatedAsync() => Task.CompletedTask;

    /// <summary>
    /// Re-evaluates the given commands whenever the session changes, so buttons enable
    /// and disable as the user signs in or selects a project. Wiring this once here
    /// removes the repeated <c>Session.PropertyChanged += ...</c> boilerplate.
    /// </summary>
    protected void WireSessionToCommands(params IRelayCommand[] commands) =>
        Session.PropertyChanged += (_, _) =>
        {
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        };

    /// <summary>
    /// Runs a use case that yields a value-carrying <see cref="Result{T}"/>, mapping a
    /// failure to its error message and a success to <paramref name="success"/>, and
    /// toggling <see cref="IsBusy"/> around the call. Returns whether it succeeded.
    /// </summary>
    protected async Task<bool> RunAsync<T>(Func<Task<Result<T>>> action, Func<T, string> success)
    {
        IsBusy = true;
        try
        {
            var result = await action();
            Status = result.IsFailure ? result.Error.Message : success(result.Value);
            return result.IsSuccess;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Runs a use case that yields a non-generic <see cref="Result"/>, mapping a failure
    /// to its error message and a success to <paramref name="success"/>.
    /// </summary>
    protected async Task<bool> RunAsync(Func<Task<Result>> action, string success)
    {
        IsBusy = true;
        try
        {
            var result = await action();
            Status = result.IsFailure ? result.Error.Message : success;
            return result.IsSuccess;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
