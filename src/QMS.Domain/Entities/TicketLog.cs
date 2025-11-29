using QMS.Domain.Common;
using QMS.Domain.Enums;

namespace QMS.Domain.Entities;

public class TicketLog : BaseEntity
{
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public TicketStatus? OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public int? CounterId { get; set; }
    public string? ActionBy { get; set; } // User/Staff ID
    public string? Note { get; set; }
}
