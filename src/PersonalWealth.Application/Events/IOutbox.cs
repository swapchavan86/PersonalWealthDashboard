using PersonalWealth.Domain.Events;

namespace PersonalWealth.Application.Events;

public interface IOutbox
{
    void Add(IDomainEvent domainEvent, Guid? tenantId = null);
}
