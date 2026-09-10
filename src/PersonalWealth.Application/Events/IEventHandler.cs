using PersonalWealth.Domain.Events;

namespace PersonalWealth.Application.Events;

public interface IEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
