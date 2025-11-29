using QMS.Domain.Common;

namespace QMS.Domain.Entities;

public class CounterAssignmentHistory : BaseEntity
{
    public int CounterId { get; set; }
    public Counter? Counter { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedAt { get; set; }
    
    public string AssignedBy { get; set; } = string.Empty; // Who assigned (TM or Manager)
    public string? UnassignedBy { get; set; }
    
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true; // Current assignment or historical
}
