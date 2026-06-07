using System.Globalization;
using System.Windows.Data;

namespace TaskFlow.Desktop.Converters;

/// <summary>
/// Returns <c>true</c> when the active section equals a button's own section, letting
/// a single sidebar style highlight the current item. The first bound value is the
/// active section; the second is the button's section (its command parameter).
/// </summary>
public sealed class SectionActiveConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture) =>
        values.Length == 2 && values[0] is string active && values[1] is string section &&
        string.Equals(active, section, StringComparison.Ordinal);

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
