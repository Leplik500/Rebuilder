using System.Security.Claims;
using Template.Net.Microservice.ThreeTier.PL.Definitions.Identity;

namespace User.Infrastructure;

public class UserContext : IUserContext
{
    public IEnumerable<Claim> Claims
    {
        get => UserIdentity.Instance.Claims;
    }

    public bool IsAuthenticated
    {
        get => this.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier);
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = this.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier
            );
            if (userIdClaim == null)
                return null;

            return Guid.TryParse(userIdClaim.Value, out var guid) ? guid : null;
        }
    }
}
