using System.Text.Json;
using PersonalWealth.Application.Events;
using PersonalWealth.Domain.Events;

namespace PersonalWealth.Infrastructure.Persistence.Outbox;

public sealed class EfCoreOutbox(PersonalWealthDbContext dbContext) : IOutbox
{
    public void Add(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
        var message = OutboxMessage.Create(
            domainEvent.EventId,
            dbContext.CurrentTenantId,
            domainEvent.GetType().AssemblyQualifiedName
                ?? throw new InvalidOperationException("Domain event type must have an assembly-qualified name."),
            payload,
            domainEvent.OccurredAtUtc,
            DateTimeOffset.UtcNow);

        dbContext.OutboxMessages.Add(message);
    }
}
