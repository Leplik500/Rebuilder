using OpenIddict.Server.AspNetCore;

namespace PublicWorkout.UI.Api;

/// <summary>
/// Data for authorization
/// </summary>
public class AuthData
{
    /// <summary>
    /// Schemes for authorization filter
    /// </summary>
    public const string AuthenticationSchemes =
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
}
