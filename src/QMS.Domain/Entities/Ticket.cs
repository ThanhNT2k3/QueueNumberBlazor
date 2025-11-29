using QMS.Domain.Common;
using QMS.Domain.Enums;

namespace QMS.Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public ServiceType? ServiceType { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int? CounterId { get; set; }
    public Counter? Counter { get; set; }
    public int? StaffId { get; set; }
    public User? Staff { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Waiting;
    public DateTime? CalledAt { get; set; }
    public DateTime? ServedAt { get; set; }
    public DateTime? StartServiceTime { get; set; }
    public DateTime? EndServiceTime { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Language { get; set; } // "en" or "vi"
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerNotes { get; set; }
    public string? Remarks { get; set; }
}
