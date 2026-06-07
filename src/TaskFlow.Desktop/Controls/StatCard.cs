using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaskFlow.Desktop.Controls;

/// <summary>
/// A KPI tile for the report dashboards: a coloured accent strip, a small caption, a
/// large value, and optional content (such as a progress bar) shown below the value.
/// The visual template lives in the merged theme dictionary.
/// </summary>
public sealed class StatCard : ContentControl
{
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(StatCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(StatCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty AccentProperty =
        DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(StatCard), new PropertyMetadata(null));

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Brush? Accent
    {
        get => (Brush?)GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }
}
