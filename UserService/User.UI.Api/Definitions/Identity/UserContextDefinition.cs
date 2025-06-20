// <copyright file="UserContextDefinition.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;
using User.Infrastructure;

namespace User.UI.Api.Definitions.Identity;

public class UserContextDefinition : ApplicationDefinition
{
    public override async Task ConfigureServicesAsync(
        IDefinitionServiceContext context
    )
    {
        context.ServiceCollection.AddScoped<IUserContext, UserContext>();
        await base.ConfigureServicesAsync(context);
    }
}
