using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Banking;

public enum BankTransactionDirection
{
    Debit,
    Credit
}

public sealed class BankTransaction : TenantEntity<Guid>, IAuditableEntity, IConcurrencyTracked
{
    private BankTransaction()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public BankTransaction(Guid id, Guid tenantId, Guid accountId, DateTime transactionDate, decimal amount, BankTransactionDirection direction, string description, Guid importId, string sourceFingerprint)
        : base(id, tenantId)
    {
        if (accountId == Guid.Empty) throw new ArgumentException("AccountId must not be empty.", nameof(accountId));
        if (transactionDate == default) throw new ArgumentException("Transaction date is required.", nameof(transactionDate));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (importId == Guid.Empty) throw new ArgumentException("ImportId must not be empty.", nameof(importId));
        if (string.IsNullOrWhiteSpace(sourceFingerprint)) throw new ArgumentException("Source fingerprint is required.", nameof(sourceFingerprint));
        AccountId = accountId;
        TransactionDate = transactionDate.Date;
        Amount = amount;
        Direction = direction;
        Description = description.Trim();
        ImportId = importId;
        SourceFingerprint = sourceFingerprint.Trim();
    }

    public Guid AccountId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public decimal Amount { get; private set; }
    public BankTransactionDirection Direction { get; private set; }
    public string Description { get; private set; } = null!;
    public Guid ImportId { get; private set; }
    public string SourceFingerprint { get; private set; } = null!;
    public string? Category { get; private set; }
    public Guid? TransferId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public void SetCategory(string? category) => Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();
    public void LinkTransfer(Guid transferId) => TransferId = transferId == Guid.Empty ? throw new ArgumentException("TransferId must not be empty.", nameof(transferId)) : transferId;
}
