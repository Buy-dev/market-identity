using FluentValidation.Results;
using Market.Identity.Application.Helpers;

namespace Market.Identity.Application.Infrastructure.Mappers;

public class ValidationErrorMapper : IMapWith<ValidationFailure, ValidationError>
{
    public ValidationError Map(ValidationFailure source)
    {
        return new ValidationError
        {
            PropertyName = source.PropertyName,
            Message = source.ErrorMessage
        };
    }
}