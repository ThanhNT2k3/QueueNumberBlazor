using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using QMS.Web.Models;

namespace QMS.Web.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _protectedSessionStore;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ProtectedSessionStorage protectedSessionStore)
    {
        _protectedSessionStore = protectedSessionStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            Console.WriteLine("[Auth] Checking session from ProtectedSessionStorage...");
            // Read session from storage
            // Note: This might throw during prerendering, which is expected and handled by catch
            var result = await _protectedSessionStore.GetAsync<SessionData>("user_session");
            
            if (result.Success && result.Value != null)
            {
                var session = result.Value;
                Console.WriteLine($"[Auth] Session found: {session.UserName} ({session.UserRole})");
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                    new Claim(ClaimTypes.Name, session.UserName),
                    new Claim(ClaimTypes.Role, session.UserRole)
                };

                if (session.BranchId > 0)
                {
                    claims.Add(new Claim("BranchId", session.BranchId.ToString()));
                }

                var identity = new ClaimsIdentity(claims, "CustomAuth");
                var principal = new ClaimsPrincipal(identity);
                
                return new AuthenticationState(principal);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] Error reading session (expected during prerendering): {ex.Message}");
        }

        return new AuthenticationState(_anonymous);
    }

    public void MarkUserAsAuthenticated(SessionData session)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
            new Claim(ClaimTypes.Name, session.UserName),
            new Claim(ClaimTypes.Role, session.UserRole)
        };
        
        if (session.BranchId > 0)
        {
            claims.Add(new Claim("BranchId", session.BranchId.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var principal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _protectedSessionStore.DeleteAsync("user_session");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
