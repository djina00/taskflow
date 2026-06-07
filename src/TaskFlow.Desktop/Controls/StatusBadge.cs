using System.Windows;
using System.Windows.Controls;

namespace TaskFlow.Desktop.Controls;

/// <summary>
/// A coloured pill showing a task's workflow state (Todo / InProgress / Done). The
/// colour for each state comes from the reusable status palette in the theme, so the
/// list and the detail dialog render the same badge from one source of truth. The
/// visual template lives in the merged theme dictionary.
/// </summary>
public sealed class StatusBadge : Control
{
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(string), typeof(StatusBadge), new PropertyMetadata(string.Empty));

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
}
