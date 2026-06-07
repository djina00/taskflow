using System.Windows;
using System.Windows.Controls;

namespace TaskFlow.Desktop.Controls;

/// <summary>
/// A single report metric: a muted caption on the left and its emphasised value on
/// the right. Replaces the parallel label/value column stacks the report cards used.
/// </summary>
public partial class MetricRow : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(MetricRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(MetricRow), new PropertyMetadata(string.Empty));

    public MetricRow() => InitializeComponent();

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
