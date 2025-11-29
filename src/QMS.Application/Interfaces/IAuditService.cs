using QMS.Domain.Enums;

namespace QMS.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string userId, string userName, AuditAction action, string entityName, string entityId, string? oldValues, string? newValues, string? details);
}
