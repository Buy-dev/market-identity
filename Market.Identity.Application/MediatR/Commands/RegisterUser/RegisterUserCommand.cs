using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<Result<object>>
{
    public string Username { get; init; }
    public string FullName { get; init; }
    public string CallSign { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}

public class RegisterUserCommandHandler(IIdentityDbContext context,
                                        IPasswordHasher<User> passwordHasher) 
    : IRequestHandler<RegisterUserCommand, Result<object>>
{
    public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            CallSign = request.CallSign,
            PasswordHash = passwordHasher.HashPassword(null, request.Password)
        };

        context.Users.Add(user);
        var isSaveSuccess = await context.SaveAsync(cancellationToken).ConfigureAwait(false);
        
        return !isSaveSuccess 
            ? Result<object>.Failure("Не удалось создать пользователя") 
            : Result<object>.Success();
    }
}