using Pepegov.MicroserviceFramework.AspNetCore.WebApplicationDefinition;
using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;
using User.Application.Dtos;
using User.Application.Services.Interfaces;

namespace User.UI.Api.EndPoints;

public class UserEndPoints : ApplicationDefinition
{
    public override Task ConfigureApplicationAsync(
        IDefinitionApplicationContext context
    )
    {
        var app = context.Parse<WebDefinitionApplicationContext>().WebApplication;
        var group = app.MapGroup("/api/user")
            .WithOpenApi()
            .WithTags("User")
            .RequireAuthorization();

        group.MapGet("profile", GetCurrentUserProfile);
        group.MapPatch("profile", UpdateUserProfile);
        group.MapGet("settings", GetUserSettings);
        group.MapPatch("settings", UpdateUserSettings);

        return base.ConfigureApplicationAsync(context);
    }

    private static async Task<IResult> UpdateUserSettings(
        UpdateUserSettingsDto updateDto,
        IUserService userService
    )
    {
        var result = await userService.UpdateUserSettingsAsync(updateDto);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> GetUserSettings(IUserService userService)
    {
        var result = await userService.GetUserSettingsAsync();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> UpdateUserProfile(
        UpdateUserProfileDto updateDto,
        IUserService userService
    )
    {
        var result = await userService.UpdateUserProfileAsync(updateDto);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> GetCurrentUserProfile(
        IUserService userService
    )
    {
        var result = await userService.GetCurrentUserProfileAsync();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }
}
