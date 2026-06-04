using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Notifications.Application.Commands.MarkNotificationAsRead;
using TaskFlow.Modules.Notifications.Application.Contracts;
using TaskFlow.Modules.Notifications.Application.Queries.GetNotificationsForRecipient;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the Notifications tab: lists the signed-in user's notifications — created
/// reactively by the event handlers when tasks are assigned or completed — and
/// marks them read.
/// </summary>
public sealed class NotificationsViewModel : ViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;
    private readonly SessionContext _session;

    private string _status = string.Empty;
    private NotificationDto? _selectedNotification;

    public NotificationsViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session)
    {
        _commands = commands;
        _queries = queries;
        _session = session;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => _session.IsLoggedIn);
        MarkReadCommand = new AsyncRelayCommand(MarkReadAsync, () => _selectedNotification is not null);
        _session.PropertyChanged += (_, _) => ((AsyncRelayCommand)RefreshCommand).RaiseCanExecuteChanged();
    }

    public SessionContext Session => _session;

    public ObservableCollection<NotificationDto> Notifications { get; } = new();

    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public NotificationDto? SelectedNotification
    {
        get => _selectedNotification;
        set
        {
            if (SetProperty(ref _selectedNotification, value))
                ((AsyncRelayCommand)MarkReadCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand MarkReadCommand { get; }

    /// <summary>
    /// Called when a notification was just created elsewhere in the app. If it is
    /// addressed to the signed-in user, the list reloads so the new item appears
    /// without a manual refresh. Marshalling onto the UI thread is the caller's job.
    /// </summary>
    public void OnNotificationCreated(Guid recipientId)
    {
        if (recipientId == _session.CurrentUserId)
            _ = RefreshAsync();
    }

    private async Task MarkReadAsync()
    {
        if (_selectedNotification is null)
            return;

        var result = await _commands.SendAsync(new MarkNotificationAsReadCommand(_selectedNotification.Id));
        Status = result.IsFailure ? result.Error.Message : "Marked as read.";
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        if (_session.CurrentUserId is not Guid userId)
        {
            Notifications.Clear();
            return;
        }

        var notifications = await _queries.QueryAsync(new GetNotificationsForRecipientQuery(userId));
        Notifications.Clear();
        foreach (var notification in notifications)
            Notifications.Add(notification);
    }
}
