namespace Market.Identity.Application;

public record ValidationError
{
    public string PropertyName { get; init; }
    public string Message { get; init; }
}