using Microsoft.AspNetCore.SignalR;
using QMS.Web.Hubs;
using QMS.Application.Interfaces;

namespace QMS.Web.Services;

public class QmsNotificationService : IQmsNotificationService
{
    private readonly IHubContext<QmsHub> _hubContext;
    private readonly ILogger<QmsNotificationService> _logger;
    private readonly QmsEventService _eventService;

    public QmsNotificationService(IHubContext<QmsHub> hubContext, ILogger<QmsNotificationService> logger, QmsEventService eventService)
    {
        _hubContext = hubContext;
        _logger = logger;
        _eventService = eventService;
    }

    public async Task NotifyCounterUpdatedAsync(int counterId)
    {
        _logger.LogInformation("[NotificationService] Sending CounterUpdated to counter_{CounterId}", counterId);
        
        // 1. Send to SignalR clients
        await _hubContext.Clients.Group($"counter_{counterId}")
            .SendAsync("CounterUpdated", counterId);
            
        // 2. Trigger global event via Singleton service
        _eventService.TriggerCounterUpdated(counterId, true);
        
        _logger.LogInformation("[NotificationService] CounterUpdated sent to counter_{CounterId}", counterId);
    }

    public async Task NotifyTicketUpdatedAsync(int branchId, int counterId)
    {
        _logger.LogInformation("[NotificationService] Sending TicketUpdated to counter_{CounterId}", counterId);
        await _hubContext.Clients.Group($"counter_{counterId}")
            .SendAsync("TicketUpdated", counterId);
        
        _logger.LogInformation("[NotificationService] Sending BranchTicketUpdated to branch_{BranchId}", branchId);
        await _hubContext.Clients.Group($"branch_{branchId}")
            .SendAsync("BranchTicketUpdated", branchId);
    }

    public async Task NotifyCounterStatusChangedAsync(int counterId, string status)
    {
        _logger.LogInformation("[NotificationService] Sending CounterStatusChanged to counter_{CounterId} with status: {Status}", counterId, status);
        
        // Send to counter group
        await _hubContext.Clients.Group($"counter_{counterId}")
            .SendAsync("CounterStatusChanged", counterId, status);
            
        _logger.LogInformation("[NotificationService] CounterStatusChanged sent to counter_{CounterId}", counterId);
    }

    public async Task NotifyCounterUpdatedAsync(int counterId, int branchId, bool isActive)
    {
        _logger.LogInformation("[NotificationService] Sending CounterUpdated to branch_{BranchId} for counter {CounterId}, Active: {IsActive}", branchId, counterId, isActive);
        
        // 1. Send to branch group for TM to receive (if they are on separate client)
        await _hubContext.Clients.Group($"branch_{branchId}")
            .SendAsync("CounterUpdated", counterId, isActive);
            
        // 2. Trigger global event via Singleton service
        _eventService.TriggerCounterUpdated(counterId, isActive);
            
        _logger.LogInformation("[NotificationService] CounterUpdated sent to branch_{BranchId}", branchId);
    }

    public async Task NotifyCounterAssignedAsync(int branchId, int counterId, int userId, string userName)
    {
        _logger.LogInformation("[NotificationService] Sending CounterAssigned to branch_{BranchId}", branchId);
        await _hubContext.Clients.Group($"branch_{branchId}")
            .SendAsync("CounterAssigned", counterId, userId, userName);
            
        // Trigger global event
        _eventService.TriggerCounterAssigned(counterId, userId, userName);
    }

    public async Task NotifyCounterUnassignedAsync(int branchId, int counterId)
    {
        _logger.LogInformation("[NotificationService] Sending CounterUnassigned to branch_{BranchId}", branchId);
        await _hubContext.Clients.Group($"branch_{branchId}")
            .SendAsync("CounterUnassigned", counterId);
            
        // Trigger global event
        _eventService.TriggerCounterUnassigned(counterId);
    }

    // These methods are for Blazor components to call
    public Task JoinBranchGroupAsync(int branchId)
    {
        // This will be handled by the component's own SignalR connection
        // Not needed in server-side service
        return Task.CompletedTask;
    }

    public Task LeaveBranchGroupAsync(int branchId)
    {
        return Task.CompletedTask;
    }

    public Task JoinCounterGroupAsync(int counterId)
    {
        return Task.CompletedTask;
    }

    public Task LeaveCounterGroupAsync(int counterId)
    {
        return Task.CompletedTask;
    }
}
