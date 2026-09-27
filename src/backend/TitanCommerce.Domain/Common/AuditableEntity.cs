
namespace TitanCommerce.Domain.Common;

public abstract class AuditableEntity<TId> : BaseEntity<TId>
{
    public DateTime CreatedAt { get; set; }
    public String? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public String? LastModifiedBy { get; set; }

    protected AuditableEntity() : base() { }
    protected AuditableEntity(TId id) : base(id) { }
}
