using QMS.Domain.Entities;

namespace QMS.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    Task<IEnumerable<User>> GetTellersByBranchAsync(int branchId);
}
