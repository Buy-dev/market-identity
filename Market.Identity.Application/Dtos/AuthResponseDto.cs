namespace Market.Identity.Application.Dtos;

/// <summary>
/// Ответ на запрос авторизации
/// </summary>
public record AuthResponseDto
{
    /// <summary>
    /// Access Токен
    /// </summary>
    public string AccessToken { get; init; }
    
    /// <summary>
    /// Refresh Токен
    /// </summary>
    public string RefreshToken { get; init; }

    /// <summary>
    /// Срок действия токена
    /// </summary>
    public DateTime Expires { get; init; }
}