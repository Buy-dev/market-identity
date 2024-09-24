namespace Market.Identity.Application.Dtos;

/// <summary>
/// Запрос на обновление токена
/// </summary>
public record RefreshTokenRequestDto
{
    /// <summary>
    /// Access Токен
    /// </summary>
    public string AccessToken { get; init; }
    
    /// <summary>
    /// Refresh Токен
    /// </summary>
    public string RefreshToken { get; init; }

}