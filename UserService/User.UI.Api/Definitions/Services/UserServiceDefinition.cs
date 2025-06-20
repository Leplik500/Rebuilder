using Pepegov.MicroserviceFramework.Definition.Context;
using User.Application.Services;
using User.Application.Services.Interfaces;
using User.UI.Api.Definitions.Database;

namespace User.UI.Api.Definitions.Services;

public class UserServiceDefinition : DatabaseDefinition
{
    public override async Task ConfigureServicesAsync(
        IDefinitionServiceContext context
    )
    {
        context.ServiceCollection.AddScoped<IUserService, UserService>();
        await base.ConfigureServicesAsync(context);
    }
}
