using PersonalWealth.Domain.Entities;
using Xunit;

namespace PersonalWealth.UnitTests.Domain.Entities;

public sealed class TenantEntityContractTests
{
    [Fact]
    public void Tenant_id_is_required_and_exposed()
    {
        var tenantId = Guid.NewGuid();
        var entity = new TestTenantEntity(Guid.NewGuid(), tenantId);

        Assert.Equal(tenantId, entity.TenantId);
        Assert.Equal(tenantId, ((ITenantOwned)entity).TenantId);
    }

    [Fact]
    public void Empty_tenant_id_is_rejected()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TestTenantEntity(Guid.NewGuid(), Guid.Empty));

        Assert.Equal("tenantId", exception.ParamName);
    }

    [Fact]
    public void Tenant_id_remains_stable()
    {
        var tenantId = Guid.NewGuid();
        var entity = new TestTenantEntity(Guid.NewGuid(), tenantId);

        Assert.Equal(tenantId, entity.TenantId);
    }

    [Fact]
    public void Tenant_id_does_not_change_entity_identity_or_equality()
    {
        var entityId = Guid.NewGuid();

        var first = new TestTenantEntity(entityId, Guid.NewGuid());
        var second = new TestTenantEntity(entityId, Guid.NewGuid());

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Different_tenant_ids_are_represented_without_changing_entity_identity()
    {
        var entityId = Guid.NewGuid();
        var firstTenant = Guid.NewGuid();
        var secondTenant = Guid.NewGuid();

        var first = new TestTenantEntity(entityId, firstTenant);
        var second = new TestTenantEntity(entityId, secondTenant);

        Assert.Equal(firstTenant, first.TenantId);
        Assert.Equal(secondTenant, second.TenantId);
        Assert.Equal(first, second);
    }

    private sealed class TestTenantEntity(Guid id, Guid tenantId)
        : TenantEntity<Guid>(id, tenantId);
}
