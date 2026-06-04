using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace TaskFlow.Desktop.Converters;

/// <summary>Joins an <see cref="IEnumerable"/> of values into a comma-separated string for display.</summary>
public sealed class JoinStringsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is IEnumerable items and not string
            ? string.Join(", ", items.Cast<object?>())
            : value?.ToString() ?? string.Empty;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
