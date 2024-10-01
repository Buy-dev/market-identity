using System.Collections;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Market.Identity.Application.Infrastructure.Validation;

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        var validationErrors = validationProblemDetails!.Errors.Select(e => new ValidationError
        {
            PropertyName = e.Key,
            Message = string.Join(", ", e.Value)
        })
        .ToList();

        var response = Result<IEnumerable<ValidationError>>.ValidationError(validationErrors);
        
        return new BadRequestObjectResult(response);
    }
}