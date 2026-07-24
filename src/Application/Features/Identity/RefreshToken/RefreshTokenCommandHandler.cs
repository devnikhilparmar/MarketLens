namespace MarketLens.Application.Features.Identity.RefreshToken;

using MarketLens.Application.Abstractions.Identity;
using MarketLens.Application.Abstractions.Messaging;
using MarketLens.Domain.Common;

public sealed class RefreshTokenCommandHandler(ITokenService tokenService) : ICommandHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await tokenService.RefreshTokenAsync(command.AccessToken, command.RefreshToken, cancellationToken);
            return Result.Success(token);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<TokenResponse>(Error.Validation("Auth.InvalidRefreshToken", ex.Message));
        }
    }
}
