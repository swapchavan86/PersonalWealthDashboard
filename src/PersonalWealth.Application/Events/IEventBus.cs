using PersonalWealth.Domain.Events;

namespace PersonalWealth.Application.Events;

public interface IEventBus
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
