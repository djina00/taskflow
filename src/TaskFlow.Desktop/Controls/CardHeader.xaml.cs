using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskFlow.Desktop.Controls;

/// <summary>
/// The header bar repeated atop the content cards: a title on the left and an
/// optional action button (defaulting to "Refresh") on the right. The button hides
/// itself when no <see cref="ActionCommand"/> is bound.
/// </summary>
public partial class CardHeader : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CardHeader), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ActionCommandProperty =
        DependencyProperty.Register(nameof(ActionCommand), typeof(ICommand), typeof(CardHeader), new PropertyMetadata(null));

    public static readonly DependencyProperty ActionTextProperty =
        DependencyProperty.Register(nameof(ActionText), typeof(string), typeof(CardHeader), new PropertyMetadata("Refresh"));

    public CardHeader() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => (ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public string ActionText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }
}
