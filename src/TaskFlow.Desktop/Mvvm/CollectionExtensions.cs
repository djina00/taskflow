using System.Collections.ObjectModel;

namespace TaskFlow.Desktop.Mvvm;

/// <summary>Helpers for the observable collections that back the data-bound lists.</summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Replaces every item in the collection with the supplied ones in a single
    /// refresh. Centralises the clear-then-repopulate that each feature view model
    /// would otherwise hand-roll after re-querying the read side.
    /// </summary>
    public static void ReplaceAll<T>(this ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }
}
