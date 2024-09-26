using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<Result<object>>
{
    public string Username { get; set; }
    public string FullName { get; set; }
    public string CallSign { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterUserCommandHandler(IIdentityDbContext context,
                                        RegisterUserCommandValidator validator,
                                        IPasswordHasher<User> passwordHasher, 
                                        IEmailService emailService) 
    : IRequestHandler<RegisterUserCommand, Result<object>>
{
    public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if(!validationResult.IsValid)
            return Result<object>.Failure(validationResult.Errors);
        
        var existingUser = await context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email || x.Username == request.Username, cancellationToken);
        
        if(existingUser != null)
            return Result<object>.Failure("User with this email or username already exists");
        
        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            CallSign = request.CallSign,
            PasswordHash = passwordHasher.HashPassword(null, request.Password)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        // Send email confirmation
        var confirmationToken = GenerateEmailConfirmationToken(user);
        await _emailService.SendEmailAsync(user.Email, "Confirm your account", $"Confirm token: {confirmationToken}");

        return Result<object>.Success();
    }
    
    private string GenerateEmailConfirmationToken(User user)
    {
        Random rnd = new Random();
        var code = rnd.Next(10000, 99999);
        return code.ToString();
    }
}