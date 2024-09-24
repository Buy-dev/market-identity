namespace Market.Identity.Application;

public record ValidationError
{
    public string FieldName { get; set; }
    public string Message { get; set; }
}