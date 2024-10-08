namespace Market.Identity.Domain.Entities.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime Created { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime Modified { get; set; }
    public Guid ModifiedBy { get; set; }
}