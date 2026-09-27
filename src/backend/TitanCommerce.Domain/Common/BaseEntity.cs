
namespace TitanCommerce.Domain.Common;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    protected BaseEntity() { }
    protected BaseEntity(TId id)
    {
        Id = id;
    }
}