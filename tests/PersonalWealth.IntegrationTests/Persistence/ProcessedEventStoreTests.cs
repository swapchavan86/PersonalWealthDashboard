using Microsoft.EntityFrameworkCore;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Persistence.ProcessedEvents;
using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class ProcessedEventStoreTests
{
    [Fact]
    public async Task Processed_event_identity_is_persisted_and_duplicate_is_ignored()
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var tenantContext = new TenantContext(Guid.NewGuid());

        await using var context = new TestDbContext(options, tenantContext);
        var store = new EfCoreProcessedEventStore(context);
        var eventId = Guid.NewGuid();

        Assert.False(await store.HasProcessedAsync(eventId));
        await store.MarkProcessedAsync(eventId, "Test.Event", DateTimeOffset.UtcNow);
        await store.MarkProcessedAsync(eventId, "Test.Event", DateTimeOffset.UtcNow);

        Assert.True(await store.HasProcessedAsync(eventId));
        Assert.Single(await context.ProcessedEvents.ToListAsync());
    }

    private sealed class TestDbContext(
        DbContextOptions<PersonalWealthDbContext> options,
        TenantContext tenantContext)
        : PersonalWealthDbContext(options, tenantContext);
}
