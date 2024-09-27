using Market.Identity.Domain.Entities.Common;

namespace Market.Identity.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
}