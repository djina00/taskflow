using System.Windows;
using System.Windows.Controls;

namespace TaskFlow.Desktop.Controls;

/// <summary>
/// A captioned form input: the <see cref="Label"/> sits above whatever input the
/// caller supplies as content (a text box, combo box, password box…). It is a
/// content control so a call site reads simply as
/// <c>&lt;ctrl:LabeledField Label="Name"&gt;&lt;TextBox … /&gt;&lt;/ctrl:LabeledField&gt;</c>.
/// The visual template lives in the merged theme dictionary.
/// </summary>
public sealed class LabeledField : ContentControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(LabeledField), new PropertyMetadata(string.Empty));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
}
