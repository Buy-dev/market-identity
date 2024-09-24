namespace Market.Identity.Application.Dtos;

public record LoginDto
{
    public string Username { get; init; }
    public string Password { get; init; }
}