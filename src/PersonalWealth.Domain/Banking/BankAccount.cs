using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Banking;

public enum BankAccountType
{
    Savings,
    Current,
    CreditCard,
    Loan,
    Other
}

public enum BankAccountStatus
{
    Active,
    Closed
}

public sealed class BankAccount : TenantEntity<Guid>, IAuditableEntity, IConcurrencyTracked
{
    private BankAccount()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public BankAccount(Guid id, Guid tenantId, string institution, string accountNumber, BankAccountType accountType, string currency)
        : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(institution)) throw new ArgumentException("Institution is required.", nameof(institution));
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.", nameof(accountNumber));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));
        Institution = institution.Trim();
        AccountNumber = accountNumber.Trim();
        AccountType = accountType;
        Currency = currency.Trim().ToUpperInvariant();
        Status = BankAccountStatus.Active;
    }

    public string Institution { get; private set; } = null!;
    public string AccountNumber { get; private set; } = null!;
    public BankAccountType AccountType { get; private set; }
    public string Currency { get; private set; } = null!;
    public BankAccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public void Close() => Status = BankAccountStatus.Closed;
}
