namespace Market.Identity.Application.MediatR.Commands.LoginUser;

public record LoginUserDto
{
    public long Id { get; set; }
    public string Username { get; init; }
    public string PasswordHash { get; init; }
    public List<string> Roles { get; set; }
}