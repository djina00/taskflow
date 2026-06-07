using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskFlow.Desktop.Controls;

/// <summary>
/// The shared chrome for the authentication windows: the TaskFlow wordmark, a
/// subtitle, the caller-supplied input fields (the content), a primary action, a
/// link to the opposite screen, and a status line for errors. The two auth windows
/// differ only in their fields and captions, so both render through this one control.
/// The visual template lives in the merged theme dictionary.
/// </summary>
public sealed class AuthCard : ContentControl
{
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(AuthCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PrimaryTextProperty =
        DependencyProperty.Register(nameof(PrimaryText), typeof(string), typeof(AuthCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PrimaryCommandProperty =
        DependencyProperty.Register(nameof(PrimaryCommand), typeof(ICommand), typeof(AuthCard), new PropertyMetadata(null));

    public static readonly DependencyProperty LinkPromptProperty =
        DependencyProperty.Register(nameof(LinkPrompt), typeof(string), typeof(AuthCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty LinkTextProperty =
        DependencyProperty.Register(nameof(LinkText), typeof(string), typeof(AuthCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty LinkCommandProperty =
        DependencyProperty.Register(nameof(LinkCommand), typeof(ICommand), typeof(AuthCard), new PropertyMetadata(null));

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(AuthCard), new PropertyMetadata(string.Empty));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string PrimaryText
    {
        get => (string)GetValue(PrimaryTextProperty);
        set => SetValue(PrimaryTextProperty, value);
    }

    public ICommand? PrimaryCommand
    {
        get => (ICommand?)GetValue(PrimaryCommandProperty);
        set => SetValue(PrimaryCommandProperty, value);
    }

    public string LinkPrompt
    {
        get => (string)GetValue(LinkPromptProperty);
        set => SetValue(LinkPromptProperty, value);
    }

    public string LinkText
    {
        get => (string)GetValue(LinkTextProperty);
        set => SetValue(LinkTextProperty, value);
    }

    public ICommand? LinkCommand
    {
        get => (ICommand?)GetValue(LinkCommandProperty);
        set => SetValue(LinkCommandProperty, value);
    }

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }
}
