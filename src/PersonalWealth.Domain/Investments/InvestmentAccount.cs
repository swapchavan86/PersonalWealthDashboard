using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Investments;

public enum InvestmentAccountType
{
    Brokerage,
    MutualFund,
    Retirement,
    Other
}

public enum InvestmentAccountStatus
{
    Active,
    Closed
}

public sealed class InvestmentAccount : TenantEntity<Guid>, IAuditableEntity, IConcurrencyTracked
{
    private InvestmentAccount()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public InvestmentAccount(Guid id, Guid tenantId, string institution, string accountNumber, InvestmentAccountType accountType, string currency)
        : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(institution)) throw new ArgumentException("Institution is required.", nameof(institution));
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.", nameof(accountNumber));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));

        Institution = institution.Trim();
        AccountNumber = accountNumber.Trim();
        AccountType = accountType;
        Currency = currency.Trim().ToUpperInvariant();
        Status = InvestmentAccountStatus.Active;
    }

    public string Institution { get; private set; } = null!;
    public string AccountNumber { get; private set; } = null!;
    public InvestmentAccountType AccountType { get; private set; }
    public string Currency { get; private set; } = null!;
    public InvestmentAccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public void Close() => Status = InvestmentAccountStatus.Closed;
}
