using FluentValidation;
using Market.Identity.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.RegisterUser;

public class RegisterUserCommandValidator(IIdentityDbContext context) : IValidator<RegisterUserCommand>
{
    public Result<List<ValidationError>> Validate(RegisterUserCommand entity)
    {
        throw new NotImplementedException();
    }
}