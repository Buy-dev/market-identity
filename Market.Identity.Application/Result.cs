namespace Market.Identity.Application;

public class Result<TData>(bool isSuccess, 
                           List<string>? errors, 
                           List<ValidationError>? validationErrors,
                           TData? data) where TData : class
{
    public bool IsSuccess { get; set; } = isSuccess;
    public List<string>? Errors { get; set; } = errors;
    public List<ValidationError>? ValidationErrors { get; set; } = validationErrors;
    public TData? Data { get; set; } = data;

    public static Result<TData> Success(TData data) => new(true, null, null, data);
    public static Result<TData> Success() => new(true, null, null, null);
    public static Result<TData> Failure(List<string> errors) => new(false, errors, null, null);
    public static Result<TData> Failure(string error) => new(false, new List<string> { error }, null, null);
    public static Result<TData> Failure() => new(false, null, null, null);
    public static Result<TData> ValidationError(List<ValidationError> validationErrors) => new(false, null, validationErrors, null);
}