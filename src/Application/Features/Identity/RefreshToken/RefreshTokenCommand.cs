namespace MarketLens.Application.Features.Identity.RefreshToken;

using MarketLens.Application.Abstractions.Identity;
using MarketLens.Application.Abstractions.Messaging;
using MarketLens.Domain.Common;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<Result<TokenResponse>>;
