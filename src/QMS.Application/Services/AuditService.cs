using QMS.Application.Interfaces;
using QMS.Domain.Entities;
using QMS.Domain.Enums;
using QMS.Domain.Interfaces;

namespace QMS.Application.Services;

public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IRepository<AuditLog> auditRepository, IUnitOfWork unitOfWork)
    {
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(string userId, string userName, AuditAction action, string entityName, string entityId, string? oldValues, string? newValues, string? details)
    {
        var log = new AuditLog
        {
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Details = details
        };

        await _auditRepository.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }
}
