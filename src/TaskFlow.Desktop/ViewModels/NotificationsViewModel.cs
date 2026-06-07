using System.Collections.ObjectModel;
using System.Linq;
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
public sealed class NotificationsViewModel : FeatureViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;

    private NotificationDto? _selectedNotification;
    private int _unreadCount;

    public NotificationsViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session)
        : base(session)
    {
        _commands = commands;
        _queries = queries;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => Session.IsLoggedIn);
        MarkReadCommand = new AsyncRelayCommand(MarkReadAsync, () => _selectedNotification is not null);
        WireSessionToCommands(RefreshCommand);
    }

    public ObservableCollection<NotificationDto> Notifications { get; } = new();

    /// <summary>
    /// Number of unread notifications for the signed-in user. Surfaced for the sidebar
    /// badge so the unread count is visible without opening the Notifications tab.
    /// </summary>
    public int UnreadCount
    {
        get => _unreadCount;
        private set => SetProperty(ref _unreadCount, value);
    }

    public NotificationDto? SelectedNotification
    {
        get => _selectedNotification;
        set
        {
            if (SetProperty(ref _selectedNotification, value))
                MarkReadCommand.RaiseCanExecuteChanged();
        }
    }

    public IRelayCommand RefreshCommand { get; }
    public IRelayCommand MarkReadCommand { get; }

    public override Task OnActivatedAsync() => RefreshAsync();

    /// <summary>
    /// Called when a notification was just created elsewhere in the app. If it is
    /// addressed to the signed-in user, the list reloads so the new item appears
    /// without a manual refresh. Marshalling onto the UI thread is the caller's job.
    /// </summary>
    public void OnNotificationCreated(Guid recipientId)
    {
        if (recipientId == Session.CurrentUserId)
            _ = RefreshAsync();
    }

    private async Task MarkReadAsync()
    {
        if (_selectedNotification is null)
            return;

        await RunAsync(
            () => _commands.SendAsync(new MarkNotificationAsReadCommand(_selectedNotification.Id)),
            "Marked as read.");
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        if (Session.CurrentUserId is not Guid userId)
        {
            Notifications.Clear();
            UnreadCount = 0;
            return;
        }

        Notifications.ReplaceAll(await _queries.QueryAsync(new GetNotificationsForRecipientQuery(userId)));
        UnreadCount = Notifications.Count(n => !n.IsRead);
    }
}
