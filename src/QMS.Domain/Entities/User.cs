using QMS.Domain.Common;

namespace QMS.Domain.Entities;

public class User : BaseEntity
{
    public string ExternalId { get; set; } = string.Empty; // OIDC sub or employee ID
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Teller, Manager, Admin
    public int? CurrentBranchId { get; set; }
    public Branch? CurrentBranch { get; set; }
    public int? CurrentCounterId { get; set; }
    public Counter? CurrentCounter { get; set; }
    public bool IsActive { get; set; } = true;
}
