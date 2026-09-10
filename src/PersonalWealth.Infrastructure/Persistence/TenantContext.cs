using PersonalWealth.Application.Tenancy;

namespace PersonalWealth.Infrastructure.Persistence;

public sealed class TenantContext : ITenantContext
{
    public TenantContext(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId must not be empty.", nameof(tenantId));
        }

        TenantId = tenantId;
    }

    public Guid TenantId { get; }
}
