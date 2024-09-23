namespace Market.Identity.Domain.Entities.Common;

public class AuditableEntity : BaseEntity
{
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}