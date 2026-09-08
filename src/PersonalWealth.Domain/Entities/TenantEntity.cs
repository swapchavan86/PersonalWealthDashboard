namespace PersonalWealth.Domain.Entities;

public interface ITenantOwned
{
    Guid TenantId { get; }
}

public abstract class TenantEntity<TId> : Entity<TId>, ITenantOwned
{
    protected TenantEntity(TId id, Guid tenantId)
        : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId must not be empty.", nameof(tenantId));
        }

        TenantId = tenantId;
    }

    public Guid TenantId { get; }
}
