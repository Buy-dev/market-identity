namespace Market.Identity.Application;

public class Result<TData>(bool isSuccess, 
                           List<string>? errors, 
                           TData? data) where TData : class
{
    public bool IsSuccess { get; set; } = isSuccess;
    public List<string>? Errors { get; set; } = errors;
    public TData? Data { get; set; } = data;

    public static Result<TData> Success(TData data) => new(true, null, data);
    public static Result<TData> Success() => new(true, null, null);
    public static Result<TData> Failure(List<string> errors) => new(false, errors, null);
    public static Result<TData> Failure(string error) => new(false, new List<string> { error }, null);
    public static Result<TData> Failure() => new(false, null, null);
}