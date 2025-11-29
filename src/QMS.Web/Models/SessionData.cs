namespace QMS.Web.Models;

public class SessionData
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string UserRole { get; set; } = "";
    public int? CounterId { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
}
