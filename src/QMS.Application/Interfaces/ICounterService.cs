using QMS.Domain.Entities;
using QMS.Domain.Enums;

namespace QMS.Application.Interfaces;

public interface ICounterService
{
    Task<Counter?> GetCounterByIdAsync(int id);
    Task<IEnumerable<Counter>> GetCountersByBranchAsync(int branchId);
    Task UpdateCounterStatusAsync(int counterId, bool isActive);
    Task ChangeCounterStatusAsync(int counterId, CounterStatus status);
    Task AssignUserToCounterAsync(int counterId, string userId);
    Task TransferUserToCounterAsync(string userId, int targetCounterId);
    
    // TM Role Methods
    Task AssignTellerToCounterAsync(int counterId, int userId, string assignedBy, string? notes = null);
    Task UnassignTellerFromCounterAsync(int counterId, string unassignedBy, string? notes = null);
    Task<IEnumerable<CounterAssignmentHistory>> GetCounterAssignmentHistoryAsync(int? counterId = null, int? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
}

