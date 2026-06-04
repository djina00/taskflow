using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskFlow.Desktop.Mvvm;

/// <summary>
/// Minimal MVVM base: change notification for data-bound view models. Keeping the
/// UI layer thin and binding-driven means the views hold no business logic — they
/// only present the view models, which in turn drive the application through the
/// CQRS dispatchers.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
