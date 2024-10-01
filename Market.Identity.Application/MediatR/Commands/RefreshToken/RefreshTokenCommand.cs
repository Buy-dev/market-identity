using Market.Identity.Application.Dtos;
using Market.Identity.Application.Services;
using MediatR;

namespace Market.Identity.Application.MediatR.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler(ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenResponse = await tokenService
            .RefreshTokensAsync(request.AccessToken, request.RefreshToken, cancellationToken)
            .ConfigureAwait(false);

        return tokenResponse is null
            ? Result<TokenResponse>.Failure("Указаны неверные токены")
            : Result<TokenResponse>.Success(tokenResponse);
    }
}