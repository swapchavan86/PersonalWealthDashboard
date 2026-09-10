using Microsoft.EntityFrameworkCore;
using PersonalWealth.Domain.Entities;
using PersonalWealth.Domain.Events;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Persistence.Outbox;
using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class TransactionalOutboxTests
{
    [Fact]
    public async Task Business_change_and_outbox_message_commit_together()
    {
        await using var database = SqlServerTestDatabase.Create();
        var tenantId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        await using (var context = CreateContext(database.ConnectionString, tenantId))
        {
            await context.Database.EnsureCreatedAsync();

            context.TenantEntities.Add(new TestTenantEntity(Guid.NewGuid(), tenantId));
            new EfCoreOutbox(context).Add(new TestEvent(eventId, DateTimeOffset.UtcNow));

            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(database.ConnectionString, tenantId))
        {
            Assert.Single(await context.TenantEntities.ToListAsync());
            var outbox = await context.OutboxMessages.SingleAsync(x => x.EventId == eventId);

            Assert.Equal(tenantId, outbox.TenantId);
            Assert.Equal(typeof(TestEvent).AssemblyQualifiedName, outbox.EventType);
            Assert.Null(outbox.PublishedAtUtc);
        }
    }

    [Fact]
    public async Task Duplicate_event_identity_rolls_back_the_business_change()
    {
        await using var database = SqlServerTestDatabase.Create();
        var tenantId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        await using (var context = CreateContext(database.ConnectionString, tenantId))
        {
            await context.Database.EnsureCreatedAsync();
            new EfCoreOutbox(context).Add(new TestEvent(eventId, DateTimeOffset.UtcNow));
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(database.ConnectionString, tenantId))
        {
            context.TenantEntities.Add(new TestTenantEntity(Guid.NewGuid(), tenantId));
            new EfCoreOutbox(context).Add(new TestEvent(eventId, DateTimeOffset.UtcNow));

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await using (var context = CreateContext(database.ConnectionString, tenantId))
        {
            Assert.Empty(await context.TenantEntities.ToListAsync());
            Assert.Single(await context.OutboxMessages.ToListAsync());
        }
    }

    private static OutboxTestDbContext CreateContext(string connectionString, Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new OutboxTestDbContext(options, new TenantContext(tenantId));
    }

    private sealed class OutboxTestDbContext(
        DbContextOptions<PersonalWealthDbContext> options,
        TenantContext tenantContext)
        : PersonalWealthDbContext(options, tenantContext)
    {
        public DbSet<TestTenantEntity> TenantEntities => Set<TestTenantEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestTenantEntity>();
            base.OnModelCreating(modelBuilder);
        }
    }

    private sealed class TestTenantEntity(Guid id, Guid tenantId)
        : TenantEntity<Guid>(id, tenantId);

    private sealed record TestEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
