using QMS.Domain.Common;

namespace QMS.Domain.Entities;

public class ServiceType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string IconClass { get; set; } = "bi-gear"; // Bootstrap icon class
    public string ColorCode { get; set; } = "#667eea"; // Hex color code
}
