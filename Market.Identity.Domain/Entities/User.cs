using Market.Identity.Domain.Entities.Common;

namespace Market.Identity.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string CallSign { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}