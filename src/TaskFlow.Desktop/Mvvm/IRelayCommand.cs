using System.Windows.Input;

namespace TaskFlow.Desktop.Mvvm;

/// <summary>
/// An <see cref="ICommand"/> whose availability can be re-evaluated on demand. Both
/// the synchronous and asynchronous relay commands implement it, so view models can
/// hold and refresh them through one type instead of casting to a concrete command.
/// </summary>
public interface IRelayCommand : ICommand
{
    /// <summary>Raises <see cref="ICommand.CanExecuteChanged"/> so bound controls re-query <see cref="ICommand.CanExecute"/>.</summary>
    void RaiseCanExecuteChanged();
}
