using QMS.Domain.Common;
using QMS.Domain.Enums;

namespace QMS.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty; // Store external ID (e.g. OIDC sub) or internal ID
    public string UserName { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? Details { get; set; }
}
