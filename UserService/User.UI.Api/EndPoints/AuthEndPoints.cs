using Pepegov.MicroserviceFramework.AspNetCore.WebApplicationDefinition;
using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;

namespace User.UI.Api.EndPoints;

public class AuthEndPoints : ApplicationDefinition
{
    public override Task ConfigureApplicationAsync(
        IDefinitionApplicationContext context
    )
    {
        var app = context.Parse<WebDefinitionApplicationContext>().WebApplication;
        app.MapPost("~/auth/request-otp", this.SendOtpToEmail)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/verify-otp", this.GetJwtToken)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/refresh", this.RefreshAccessToken)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/logout", this.RevokeRefreshToken)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/register", this.CreateUser)
            .WithOpenApi()
            .WithTags("Auth");

        return base.ConfigureApplicationAsync(context);
    }

    private Task CreateUser(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private Task RevokeRefreshToken(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private Task RefreshAccessToken(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private Task GetJwtToken(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private Task SendOtpToEmail(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
