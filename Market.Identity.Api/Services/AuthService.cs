using Grpc.Core;
using Market.Identity.Grpc;
using MediatR;

namespace Market.Identity.Services;

public class AuthService(IMediator mediator) : AuthGrpc.AuthGrpcBase
{
    public override Task<LoginResponse> Login(LoginUserRequest request, ServerCallContext context)
    {
        return base.Login(request, context);
    }
    
    public override Task<RegisterResponse> Register(RegisterUserRequest request, ServerCallContext context)
    {
        return base.Register(request, context);
    }
    
    public override Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        return base.RefreshToken(request, context);
    }
}