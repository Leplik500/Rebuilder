using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace User.Infrastructure;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public IEnumerable<Claim> Claims
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.Claims ?? [];
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated ?? false;
        }
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = this
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            // Пытаемся распарсить строку в Guid
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }
    }

    public string? UserName
    {
        get
        {
            return this.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
    }

    public string? Email
    {
        get
        {
            return this
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
        }
    }
}
