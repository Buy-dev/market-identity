namespace Market.Identity.Application.Services;

public interface IValidator<in T> where T : class
{
    public Result<List<ValidationError>> Validate(T entity);
}