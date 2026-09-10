using PersonalWealth.Domain.Events;

namespace PersonalWealth.Domain.Banking;

public sealed record BankTransactionImportedEvent(
    Guid EventId,
    DateTimeOffset OccurredAtUtc,
    Guid TenantId,
    Guid TransactionId,
    Guid ImportId) : IDomainEvent;

public sealed record BankImportCommittedEvent(
    Guid EventId,
    DateTimeOffset OccurredAtUtc,
    Guid TenantId,
    Guid ImportId,
    int TransactionCount) : IDomainEvent;
