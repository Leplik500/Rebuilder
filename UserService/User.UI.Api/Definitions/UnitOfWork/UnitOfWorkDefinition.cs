﻿using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;
using Pepegov.UnitOfWork;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using User.Application;
using User.Application.Database;

namespace User.UI.Api.Definitions.UnitOfWork;

/// <summary>
/// Registration unit of work db providers
/// </summary>
public class UnitOfWorkDefinition : ApplicationDefinition
{
    /// <inheritdoc />
    public override Task ConfigureServicesAsync(
        IDefinitionServiceContext serviceContext
    )
    {
        serviceContext.ServiceCollection.AddUnitOfWork(x =>
        {
            x.UsingEntityFramework(
                (context, configurator) =>
                {
                    configurator.DatabaseContext<ApplicationDbContext>();
                }
            );
        });

        return base.ConfigureServicesAsync(serviceContext);
    }
}
