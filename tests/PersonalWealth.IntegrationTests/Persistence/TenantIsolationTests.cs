using Microsoft.EntityFrameworkCore;
using PersonalWealth.Domain.Entities;
using PersonalWealth.Infrastructure.Persistence;
using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class TenantIsolationTests
{
    [Fact]
    public void Tenant_owned_entity_is_persisted_with_tenant_identity()
    {
        var tenantId = Guid.NewGuid();
        using var context = CreateContext(tenantId);
        context.TenantEntities.Add(new TestTenantEntity(Guid.NewGuid(), tenantId));

        context.SaveChanges();

        Assert.Equal(tenantId, context.TenantEntities.Single().TenantId);
    }

    [Fact]
    public void Tenant_cannot_read_or_mutate_another_tenants_entity()
    {
        var entityId = Guid.NewGuid();
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        using (var context = CreateContext(tenantA))
        {
            context.TenantEntities.Add(new TestTenantEntity(entityId, tenantA));
            context.SaveChanges();
        }

        using (var context = CreateContext(tenantB))
        {
            Assert.Empty(context.TenantEntities);

            var foreignEntity = new TestTenantEntity(entityId, tenantA);
            context.TenantEntities.Attach(foreignEntity);
            foreignEntity.Name = "tampered";

            Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
        }

        using (var context = CreateContext(tenantB))
        {
            context.TenantEntities.Remove(new TestTenantEntity(entityId, tenantA));

            Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
        }
    }

    [Fact]
    public void Missing_or_invalid_tenant_context_fails_safely()
    {
        Assert.Throws<ArgumentException>(() => new TenantContext(Guid.Empty));

        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>().Options;
        Assert.Throws<ArgumentNullException>(() => new PersonalWealthDbContext(options, null!));
    }

    [Fact]
    public void Non_tenant_owned_entities_are_not_filtered()
    {
        using var context = CreateContext(Guid.NewGuid());
        context.NonTenantEntities.Add(new TestEntity(Guid.NewGuid()));
        context.SaveChanges();

        Assert.Single(context.NonTenantEntities);
        Assert.Empty(context.Model.FindEntityType(typeof(TestEntity))!.GetDeclaredQueryFilters());
    }

    private static IsolationTestDbContext CreateContext(Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new IsolationTestDbContext(options, new TenantContext(tenantId));
    }

    private sealed class IsolationTestDbContext(
        DbContextOptions<PersonalWealthDbContext> options,
        TenantContext tenantContext)
        : PersonalWealthDbContext(options, tenantContext)
    {
        public DbSet<TestTenantEntity> TenantEntities => Set<TestTenantEntity>();
        public DbSet<TestEntity> NonTenantEntities => Set<TestEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestTenantEntity>();
            modelBuilder.Entity<TestEntity>();
            base.OnModelCreating(modelBuilder);
        }
    }

    private sealed class TestTenantEntity(Guid id, Guid tenantId)
        : TenantEntity<Guid>(id, tenantId)
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestEntity(Guid id) : Entity<Guid>(id);
}
