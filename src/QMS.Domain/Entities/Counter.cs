using QMS.Domain.Common;
using QMS.Domain.Enums;

namespace QMS.Domain.Entities;

public class Counter : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty; // e.g. "C1", "VIP", "A"
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public bool IsActive { get; set; } = true;
    public CounterStatus Status { get; set; } = CounterStatus.Offline;
    public int? CurrentTicketId { get; set; }
    public int? AssignedUserId { get; set; }
    public int DailySequence { get; set; } = 0; // Reset daily
    public DateTime LastResetDate { get; set; } = DateTime.UtcNow.Date;
    
    // Navigation properties
    public ICollection<CounterServiceType> ServiceTypes { get; set; } = new List<CounterServiceType>();
}
