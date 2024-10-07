using FluentValidation;
using FluentValidation.Results;

namespace Market.Identity.Application.Infrastructure.Validation;

public class BaseValidator<TModel> : AbstractValidator<TModel> where TModel : class
{
    protected void AddValidationFailure(
        ValidationResult validationResult, 
        string errorMessage)
    {
        var failure = new ValidationFailure(typeof(TModel).Name, errorMessage);
        validationResult.Errors.Add(failure);
    }
    
    protected ValidationResult GenerateValidationResultWithFailure(
        ValidationResult validationResult,
        string errorMessage)
    {
        AddValidationFailure(validationResult, errorMessage);
        return validationResult;
    }
}