using Microsoft.AspNetCore.SignalR;

namespace QMS.Web.Hubs;

public class QmsHub : Hub
{
    private readonly ILogger<QmsHub> _logger;

    public QmsHub(ILogger<QmsHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinCounterGroup(int counterId)
    {
        _logger.LogInformation("[QmsHub] Client {ConnectionId} joining counter_{CounterId}", Context.ConnectionId, counterId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"counter_{counterId}");
        _logger.LogInformation("[QmsHub] Client {ConnectionId} successfully joined counter_{CounterId}", Context.ConnectionId, counterId);
    }

    public async Task LeaveCounterGroup(int counterId)
    {
        _logger.LogInformation("[QmsHub] Client {ConnectionId} leaving counter_{CounterId}", Context.ConnectionId, counterId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"counter_{counterId}");
    }

    public async Task JoinBranchGroup(int branchId)
    {
        _logger.LogInformation("[QmsHub] Client {ConnectionId} joining branch_{BranchId}", Context.ConnectionId, branchId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }

    public async Task LeaveBranchGroup(int branchId)
    {
        _logger.LogInformation("[QmsHub] Client {ConnectionId} leaving branch_{BranchId}", Context.ConnectionId, branchId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }
}
