using QMS.Domain.Common;

namespace QMS.Domain.Entities;

/// <summary>
/// Many-to-Many relationship between Counter and ServiceType
/// Defines which services a counter can handle
/// </summary>
public class CounterServiceType : BaseEntity
{
    public int CounterId { get; set; }
    public Counter? Counter { get; set; }
    
    public int ServiceTypeId { get; set; }
    public ServiceType? ServiceType { get; set; }
    
    /// <summary>
    /// Priority level for this service at this counter (1 = highest priority)
    /// Lower priority services can be handled if counter is idle
    /// </summary>
    public int Priority { get; set; } = 1;
    
    /// <summary>
    /// Whether this counter is a primary handler for this service
    /// </summary>
    public bool IsPrimary { get; set; } = true;
}
