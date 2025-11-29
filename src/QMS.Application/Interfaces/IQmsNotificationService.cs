namespace QMS.Application.Interfaces;

public interface IQmsNotificationService
{
    Task NotifyCounterUpdatedAsync(int counterId);
    Task NotifyTicketUpdatedAsync(int branchId, int counterId);
    Task NotifyCounterStatusChangedAsync(int counterId, string status);
    
    // New methods for Counter Management
    Task NotifyCounterUpdatedAsync(int counterId, int branchId, bool isActive);
    Task NotifyCounterAssignedAsync(int branchId, int counterId, int userId, string userName);
    Task NotifyCounterUnassignedAsync(int branchId, int counterId);
    
    // SignalR group management
    Task JoinBranchGroupAsync(int branchId);
    Task LeaveBranchGroupAsync(int branchId);
    Task JoinCounterGroupAsync(int counterId);
    Task LeaveCounterGroupAsync(int counterId);
}
