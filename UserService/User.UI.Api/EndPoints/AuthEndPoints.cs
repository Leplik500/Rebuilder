using Pepegov.MicroserviceFramework.AspNetCore.WebApplicationDefinition;
using Pepegov.MicroserviceFramework.Definition;
using Pepegov.MicroserviceFramework.Definition.Context;
using User.Application.Dtos;
using User.Application.Services.Interfaces;

namespace User.UI.Api.EndPoints;

public class AuthEndPoints : ApplicationDefinition
{
    public override Task ConfigureApplicationAsync(
        IDefinitionApplicationContext context
    )
    {
        var app = context.Parse<WebDefinitionApplicationContext>().WebApplication;
        app.MapPost("~/auth/request-otp", SendOtpToEmail)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/verify-otp", GetJwtToken).WithOpenApi().WithTags("Auth");
        app.MapPost("~/auth/refresh", RefreshAccessToken)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/logout", RevokeRefreshToken)
            .WithOpenApi()
            .WithTags("Auth");
        app.MapPost("~/auth/register", CreateUser).WithOpenApi().WithTags("Auth");

        return base.ConfigureApplicationAsync(context);
    }

    private static async Task<IResult> CreateUser(
        HttpContext context,
        IAuthService authService
    )
    {
        // Читаем данные регистрации из тела запроса
        var registration =
            await context.Request.ReadFromJsonAsync<RegisterUserDto>();
        if (registration == null)
        {
            return Results.BadRequest(
                new
                {
                    Errors = (IEnumerable<string>)
                        (string[])["Invalid registration data"],
                }
            );
        }

        // Вызываем сервис для создания пользователя
        var result = await authService.CreateUserAsync(registration);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                new { Errors = result.Errors.Select(e => e.Message) }
            );
    }

    private static async Task<IResult> RevokeRefreshToken(
        HttpContext context,
        IAuthService authService
    )
    {
        // Читаем refresh token из тела запроса
        var tokenData =
            await context.Request.ReadFromJsonAsync<RefreshTokenRequestDto>();
        if (tokenData == null || string.IsNullOrWhiteSpace(tokenData.RefreshToken))
        {
            return Results.BadRequest(
                new { Errors = new[] { "Refresh token is required" } }
            );
        }

        // Вызываем сервис для отзыва токена
        var result = await authService.RevokeRefreshTokenAsync(
            tokenData.RefreshToken
        );
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(
                new { Errors = (string[])result.Errors.Select(e => e.Message) }
            );
    }

    private static async Task<IResult> RefreshAccessToken(
        HttpContext context,
        IAuthService authService
    )
    {
        // Читаем refresh token из тела запроса
        var tokenData =
            await context.Request.ReadFromJsonAsync<RefreshTokenRequestDto>();
        if (tokenData == null || string.IsNullOrWhiteSpace(tokenData.RefreshToken))
        {
            return Results.BadRequest(
                new { Errors = new[] { "Refresh token is required" } }
            );
        }

        // Вызываем сервис для обновления токена
        var result = await authService.RefreshAccessTokenAsync(
            tokenData.RefreshToken
        );
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return Results.BadRequest(
            new { Errors = (string[])result.Errors.Select(e => e.Message) }
        );
    }

    private static async Task<IResult> GetJwtToken(
        HttpContext context,
        IAuthService authService
    )
    {
        // Читаем данные для верификации OTP из тела запроса
        var request = await context.Request.ReadFromJsonAsync<VerifyOtpRequestDto>();
        if (request == null)
        {
            return Results.BadRequest(
                new { Errors = (string[])["Invalid OTP verification data"] }
            );
        }

        // Вызываем сервис для получения JWT токена
        var result = await authService.GetJwtTokenAsync(request);
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return Results.BadRequest(
            new { Errors = (string[])result.Errors.Select(e => e.Message) }
        );
    }

    private static async Task<IResult> SendOtpToEmail(
        HttpContext context,
        IAuthService authService
    )
    {
        string[] error = ["Email is required"];
        // Читаем email из тела запроса
        var emailData = await context.Request.ReadFromJsonAsync<EmailRequestDto>();
        if (emailData == null || string.IsNullOrWhiteSpace(emailData.Email))
        {
            return Results.BadRequest(new { Errors = error });
        }

        // Вызываем сервис для отправки OTP
        var result = await authService.SendOtpToEmailAsync(emailData.Email);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(
                new { Errors = (string[])result.Errors.Select(e => e.Message) }
            );
    }
}
