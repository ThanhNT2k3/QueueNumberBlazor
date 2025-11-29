using QMS.Application.Interfaces;
using QMS.Domain.Entities;
using QMS.Domain.Enums;
using QMS.Domain.Interfaces;

namespace QMS.Application.Services;

public class CounterService : ICounterService
{
    private readonly IRepository<Counter> _counterRepository;
    private readonly IRepository<CounterAssignmentHistory> _assignmentHistoryRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly IQmsNotificationService _notificationService;

    public CounterService(
        IRepository<Counter> counterRepository, 
        IRepository<CounterAssignmentHistory> assignmentHistoryRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork, 
        IAuditService auditService,
        IQmsNotificationService notificationService)
    {
        _counterRepository = counterRepository;
        _assignmentHistoryRepository = assignmentHistoryRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<Counter?> GetCounterByIdAsync(int id)
    {
        // Use FindAsync to force a DB query instead of GetByIdAsync which might return cached entity from FindAsync
        var counters = await _counterRepository.FindAsync(c => c.Id == id);
        return counters.FirstOrDefault();
    }

    public async Task<IEnumerable<Counter>> GetCountersByBranchAsync(int branchId)
    {
        // Use FindAsyncWithIncludes to load ServiceTypes collection
        // Note: This will load CounterServiceType but not the nested ServiceType
        // The UI will need to handle null ServiceType gracefully
        return await _counterRepository.FindAsyncWithIncludes(
            c => c.BranchId == branchId,
            c => c.ServiceTypes
        );
    }

    public async Task UpdateCounterStatusAsync(int counterId, bool isActive)
    {
        var counter = await _counterRepository.GetByIdAsync(counterId);
        if (counter != null)
        {
            counter.IsActive = isActive;
            await _counterRepository.UpdateAsync(counter);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ChangeCounterStatusAsync(int counterId, CounterStatus status)
    {
        var counter = await _counterRepository.GetByIdAsync(counterId);
        if (counter != null)
        {
            var oldStatus = counter.Status;
            counter.Status = status;
            await _counterRepository.UpdateAsync(counter);
            await _unitOfWork.SaveChangesAsync();
            
            // Audit log
            await _auditService.LogAsync(
                "SYSTEM", 
                "System", 
                AuditAction.Update, 
                "Counter", 
                counterId.ToString(), 
                oldStatus.ToString(), 
                status.ToString(), 
                $"Counter status changed from {oldStatus} to {status}"
            );
        }
    }

    public async Task AssignUserToCounterAsync(int counterId, string userId)
    {
        var counter = await _counterRepository.GetByIdAsync(counterId);
        if (counter != null)
        {
            // Convert string userId to int
            if (int.TryParse(userId, out int userIdInt))
            {
                // 1. Unassign user from ANY other counters they might be assigned to
                var otherCounters = await _counterRepository.FindAsync(c => c.AssignedUserId == userIdInt && c.Id != counterId);
                foreach (var otherCounter in otherCounters)
                {
                    await UnassignTellerFromCounterAsync(otherCounter.Id, "System", $"Auto-unassigned: User logged into counter {counterId}");
                }

                // 2. Check if counter is occupied by ANOTHER user
                if (counter.AssignedUserId.HasValue && counter.AssignedUserId != userIdInt)
                {
                    // Get the name of the current user for better error message
                    var currentUser = await _userRepository.GetByIdAsync(counter.AssignedUserId.Value);
                    var currentUserName = currentUser?.FullName ?? "another teller";
                    throw new InvalidOperationException($"This counter is currently occupied by {currentUserName}. Please choose another counter or ask TM to reassign.");
                }

                // 3. Assign user to this counter
                counter.AssignedUserId = userIdInt;
                await _counterRepository.UpdateAsync(counter);
                await _unitOfWork.SaveChangesAsync();
                
                // Create assignment history record
                var history = new CounterAssignmentHistory
                {
                    CounterId = counterId,
                    UserId = userIdInt,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "Self (Login)",
                    IsActive = true
                };

                // Add to context
                await _assignmentHistoryRepository.AddAsync(history);
                await _unitOfWork.SaveChangesAsync();
                
                // Audit
                await _auditService.LogAsync(userId, "User", AuditAction.Login, "Counter", counterId.ToString(), null, userId, "Assigned user to counter");
            }
        }
    }

    public async Task TransferUserToCounterAsync(string userId, int targetCounterId)
    {
        // Logic to find current counter of user and move to new one
        // Audit
        await _auditService.LogAsync(userId, "User", AuditAction.TransferCounter, "User", userId, "OldCounter", targetCounterId.ToString(), "Transfer user to new counter");
    }

    // TM Role Methods
    public async Task AssignTellerToCounterAsync(int counterId, int userId, string assignedBy, string? notes = null)
    {
        var counter = await _counterRepository.GetByIdAsync(counterId);
        if (counter == null)
            throw new Exception("Counter not found");

        // 1. Unassign user from ANY other counters they might be assigned to
        var otherCounters = await _counterRepository.FindAsync(c => c.AssignedUserId == userId && c.Id != counterId);
        foreach (var otherCounter in otherCounters)
        {
            await UnassignTellerFromCounterAsync(otherCounter.Id, assignedBy, $"Auto-unassigned: User assigned to counter {counterId}");
        }

        // 2. Unassign any current user from this counter
        if (counter.AssignedUserId.HasValue)
        {
            await UnassignTellerFromCounterAsync(counterId, assignedBy, "Auto-unassigned for new assignment");
        }

        // Assign new user
        counter.AssignedUserId = userId;
        await _counterRepository.UpdateAsync(counter);
        await _unitOfWork.SaveChangesAsync();

        // Create assignment history record
        var history = new CounterAssignmentHistory
        {
            CounterId = counterId,
            UserId = userId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy,
            Notes = notes,
            IsActive = true
        };

        // Add to context
        await _assignmentHistoryRepository.AddAsync(history);
        await _unitOfWork.SaveChangesAsync();

        // Audit log
        await _auditService.LogAsync(
            assignedBy,
            assignedBy,
            AuditAction.AssignCounter,
            "Counter",
            counterId.ToString(),
            null,
            userId.ToString(),
            $"Assigned user {userId} to counter {counterId}. {notes}"
        );

        // Send notification
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                await _notificationService.NotifyCounterAssignedAsync(
                    counter.BranchId,
                    counterId,
                    userId,
                    user.FullName
                );
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail the operation
            Console.WriteLine($"Error sending counter assignment notification: {ex.Message}");
        }
    }

    public async Task UnassignTellerFromCounterAsync(int counterId, string unassignedBy, string? notes = null)
    {
        var counter = await _counterRepository.GetByIdAsync(counterId);
        if (counter == null)
            throw new Exception("Counter not found");

        if (!counter.AssignedUserId.HasValue)
            return; // No one assigned

        var userId = counter.AssignedUserId.Value;

        // Update counter
        counter.AssignedUserId = null;
        // Automatically set status to Offline when unassigning
        if (counter.Status != CounterStatus.Offline)
        {
            counter.Status = CounterStatus.Offline;
        }
        
        await _counterRepository.UpdateAsync(counter);
        await _unitOfWork.SaveChangesAsync();

        // Update assignment history - mark as inactive
        var activeAssignments = await _assignmentHistoryRepository.FindAsync(h => 
            h.CounterId == counterId && 
            h.UserId == userId && 
            h.IsActive);

        foreach (var assignment in activeAssignments)
        {
            assignment.IsActive = false;
            assignment.UnassignedAt = DateTime.UtcNow;
            assignment.UnassignedBy = unassignedBy;
            await _assignmentHistoryRepository.UpdateAsync(assignment);
        }

        await _unitOfWork.SaveChangesAsync();

        // Audit log
        await _auditService.LogAsync(
            unassignedBy,
            unassignedBy,
            AuditAction.UnassignCounter,
            "Counter",
            counterId.ToString(),
            userId.ToString(),
            null,
            $"Unassigned user {userId} from counter {counterId}. {notes}"
        );

        // Send notification
        try
        {
            await _notificationService.NotifyCounterUnassignedAsync(
                counter.BranchId,
                counterId
            );
        }
        catch (Exception ex)
        {
            // Log but don't fail the operation
            Console.WriteLine($"Error sending counter unassignment notification: {ex.Message}");
        }
    }

    public async Task<IEnumerable<CounterAssignmentHistory>> GetCounterAssignmentHistoryAsync(
        int? counterId = null, 
        int? userId = null, 
        DateTime? fromDate = null, 
        DateTime? toDate = null)
    {
        // Use FindAsyncWithIncludes to load related entities
        // We load all history first then filter in memory for simplicity, 
        // but in production this should be optimized to filter in DB
        var query = await _assignmentHistoryRepository.FindAsyncWithIncludes(
            h => true, 
            h => h.Counter!, 
            h => h.User!
        );

        var filtered = query.AsEnumerable();

        if (counterId.HasValue)
            filtered = filtered.Where(h => h.CounterId == counterId.Value);

        if (userId.HasValue)
            filtered = filtered.Where(h => h.UserId == userId.Value);

        if (fromDate.HasValue)
            filtered = filtered.Where(h => h.AssignedAt >= fromDate.Value);

        if (toDate.HasValue)
            filtered = filtered.Where(h => h.AssignedAt <= toDate.Value.AddDays(1)); // Add 1 day to include the end date fully

        return filtered.OrderByDescending(h => h.AssignedAt);
    }
}
