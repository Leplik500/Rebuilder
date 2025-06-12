using Pepegov.MicroserviceFramework.AspNetCore.WebApplicationDefinition;
using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;

namespace User.UI.Api.EndPoints;

public class UserEndPoints : ApplicationDefinition
{
    public override Task ConfigureApplicationAsync(
        IDefinitionApplicationContext context
    )
    {
        var app = context.Parse<WebDefinitionApplicationContext>().WebApplication;
        var group = app.MapGroup("users")
            .WithOpenApi()
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("me", (Delegate)GetUserProfile);
        group.MapPatch("me", (Delegate)UpdateUserProfile);
        group.MapGet("settings", (Delegate)GetUserSettings);
        group.MapPatch("settings", (Delegate)UpdateUserSettings);

        return base.ConfigureApplicationAsync(context);
    }

    private static Task<IResult> UpdateUserSettings(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> GetUserSettings(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> UpdateUserProfile(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> GetUserProfile(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
