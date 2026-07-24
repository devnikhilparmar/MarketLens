namespace MarketLens.Application.Features.Identity.Login;

using MarketLens.Application.Abstractions.Identity;
using MarketLens.Application.Abstractions.Messaging;
using MarketLens.Domain.Common;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<TokenResponse>>;
