namespace Market.Identity.Application.Dtos;

public record UserDto(long Id, string Username, string PasswordHash, List<string> Roles);

public record ShortenUserDto(long Id, string Username);