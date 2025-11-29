using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace QMS.Web.Controllers;

[Route("account")]
public class AccountController : Controller
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] int userId, [FromForm] string fullName, [FromForm] string role, [FromForm] int? branchId, [FromForm] string? branchName, [FromForm] int? counterId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, fullName),
            new Claim(ClaimTypes.Role, role)
        };

        if (branchId.HasValue)
        {
            claims.Add(new Claim("BranchId", branchId.Value.ToString()));
            claims.Add(new Claim("BranchName", branchName ?? ""));
        }

        if (counterId.HasValue)
        {
            claims.Add(new Claim("CounterId", counterId.Value.ToString()));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false, // Session cookie, cleared on browser close
            ExpiresUtc = DateTime.UtcNow.AddMinutes(480) // 8 hours session
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Redirect based on role
        if (role == "TM")
        {
            return Redirect("/tm/dashboard");
        }
        else if (role == "Teller" && counterId.HasValue)
        {
            return Redirect($"/counter/dashboard/{counterId}");
        }

        return Redirect("/");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}
