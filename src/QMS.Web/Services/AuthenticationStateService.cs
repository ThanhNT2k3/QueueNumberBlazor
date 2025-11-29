using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace QMS.Web.Services;

public class AuthenticationStateService : IDisposable
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigation;
    
    public event Action? OnChange;
    
    public bool IsAuthenticated { get; private set; }
    public string? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? UserRole { get; private set; }
    public int? CounterId { get; private set; }
    public int? BranchId { get; private set; }
    public string? BranchName { get; private set; }

    public AuthenticationStateService(AuthenticationStateProvider authStateProvider, NavigationManager navigation)
    {
        _authStateProvider = authStateProvider;
        _navigation = navigation;
        _authStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        
        // Initialize state
        _ = UpdateStateAsync();
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        await UpdateStateAsync();
    }

    private async Task UpdateStateAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var user = state.User;

        IsAuthenticated = user.Identity?.IsAuthenticated ?? false;
        
        if (IsAuthenticated)
        {
            UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserName = user.FindFirst(ClaimTypes.Name)?.Value;
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value;
            
            if (int.TryParse(user.FindFirst("BranchId")?.Value, out int branchId))
                BranchId = branchId;
                
            BranchName = user.FindFirst("BranchName")?.Value;
            
            if (int.TryParse(user.FindFirst("CounterId")?.Value, out int counterId))
                CounterId = counterId;
        }
        else
        {
            UserId = null;
            UserName = null;
            UserRole = null;
            CounterId = null;
            BranchId = null;
            BranchName = null;
        }

        NotifyStateChanged();
    }

    // Deprecated methods kept for compatibility but redirected
    public void Login(int userId, string userName, string userRole, int? counterId, int branchId, string? branchName = null)
    {
        // No-op, login is handled by Controller
    }

    public async Task Logout()
    {
        _navigation.NavigateTo("/account/logout", true);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        _authStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
