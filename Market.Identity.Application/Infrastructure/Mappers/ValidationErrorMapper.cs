using FluentValidation.Results;

namespace Market.Identity.Application.Infrastructure.Mappers;

public class ValidationErrorMapper : IMapWith<ValidationFailure, ValidationError>
{
    public ValidationError Map(ValidationFailure user)
    {
        return new ValidationError
        {
            PropertyName = user.PropertyName,
            Message = user.ErrorMessage
        };
    }
}