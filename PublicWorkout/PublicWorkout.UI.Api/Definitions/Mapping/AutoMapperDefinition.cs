using Pepegov.MicroserviceFramework.AspNetCore.WebApplicationDefinition;
using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;

namespace User.UI.Api.Definitions.Mapping;

/// <summary>
/// Register auto mapper
/// </summary>
public class AutoMapperDefinition : ApplicationDefinition
{
    /// <inheritdoc />
    public override Task ConfigureServicesAsync(IDefinitionServiceContext context)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        context.ServiceCollection.AddAutoMapper(assemblies);
        return base.ConfigureServicesAsync(context);
    }

    /// <inheritdoc />
    public override Task ConfigureApplicationAsync(
        IDefinitionApplicationContext context
    )
    {
        var webContext = context.Parse<WebDefinitionApplicationContext>();

        var mapper =
            webContext.ServiceProvider.GetRequiredService<AutoMapper.IConfigurationProvider>();
        if (
            webContext.WebApplication.Environment.IsDevelopment()
            || webContext.WebApplication.Environment.EnvironmentName == "Local"
        )
        {
            mapper.AssertConfigurationIsValid();
        }
        else
        {
            mapper.CompileMappings();
        }

        return base.ConfigureApplicationAsync(context);
    }
}
