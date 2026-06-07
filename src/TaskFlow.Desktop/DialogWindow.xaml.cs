using System.Windows;

namespace TaskFlow.Desktop;

/// <summary>
/// A generic modal host: its content is the dialog view model, rendered through the
/// view model's <c>DataTemplate</c>. The <see cref="Services.DialogService"/> sets the
/// data context and wires the close signal, so there is no logic here beyond init.
/// </summary>
public partial class DialogWindow : Window
{
    public DialogWindow() => InitializeComponent();
}
