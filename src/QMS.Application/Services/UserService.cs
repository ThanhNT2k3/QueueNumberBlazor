using QMS.Application.Interfaces;
using QMS.Domain.Entities;
using QMS.Domain.Interfaces;

namespace QMS.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email);
        return users.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId)
    {
        return await _userRepository.FindAsync(u => u.CurrentBranchId == branchId);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _userRepository.FindAsync(u => u.Role == role);
    }

    public async Task<IEnumerable<User>> GetTellersByBranchAsync(int branchId)
    {
        return await _userRepository.FindAsync(u => 
            u.CurrentBranchId == branchId && u.Role == "Teller");
    }
}
