using Market.Identity.Domain.Entities.Common;

namespace Market.Identity.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}