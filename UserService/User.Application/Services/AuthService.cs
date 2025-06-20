// <copyright file="AuthService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentResults;
using User.Application.Dtos;
using User.Application.Services.Interfaces;

namespace User.Application.Services;

public class AuthService : IAuthService
{
    public async Task<Result> SendOtpToEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<Result<JwtTokenDto>> GetJwtTokenAsync(
        VerifyOtpRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<Result<JwtTokenDto>> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserDto>> CreateUserAsync(
        RegisterUserDto registration,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
