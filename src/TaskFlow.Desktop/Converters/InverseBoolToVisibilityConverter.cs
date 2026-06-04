using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskFlow.Desktop.Converters;

/// <summary>Maps a <see cref="bool"/> to <see cref="Visibility"/> inverted (true → Collapsed, false → Visible).</summary>
public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Visibility.Collapsed;
}
