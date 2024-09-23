using Market.Identity.Domain.Entities.Common;

namespace Market.Identity.Domain.Entities;

public class Seller : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
}