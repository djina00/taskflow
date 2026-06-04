using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskFlow.Desktop.Converters;

/// <summary>Maps a <see cref="bool"/> to <see cref="Visibility"/> (true → Visible, false → Collapsed).</summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Visibility.Visible;
}
