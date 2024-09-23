using Market.Identity.Domain.Entities.Common;

namespace Market.Identity.Domain.Entities;

public class Buyer : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
}