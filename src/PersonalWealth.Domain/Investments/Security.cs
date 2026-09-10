using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Investments;

public enum SecurityType
{
    Equity,
    MutualFund,
    Etf,
    Bond,
    FixedIncome,
    Other
}

public sealed class Security : TenantEntity<Guid>, IAuditableEntity
{
    private Security()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public Security(Guid id, Guid tenantId, string symbol, string name, SecurityType securityType, string currency, string? isin = null)
        : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException("Symbol is required.", nameof(symbol));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Security name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));

        Symbol = symbol.Trim().ToUpperInvariant();
        Name = name.Trim();
        SecurityType = securityType;
        Currency = currency.Trim().ToUpperInvariant();
        Isin = string.IsNullOrWhiteSpace(isin) ? null : isin.Trim().ToUpperInvariant();
        IsActive = true;
    }

    public string Symbol { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public SecurityType SecurityType { get; private set; }
    public string Currency { get; private set; } = null!;
    public string? Isin { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
