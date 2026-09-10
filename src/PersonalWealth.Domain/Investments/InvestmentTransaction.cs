using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Investments;

public enum InvestmentTransactionType
{
    Buy,
    Sell,
    Dividend,
    Fee
}

public sealed class InvestmentTransaction : TenantEntity<Guid>, IAuditableEntity
{
    private InvestmentTransaction()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public InvestmentTransaction(
        Guid id,
        Guid tenantId,
        Guid investmentAccountId,
        Guid securityId,
        DateTime transactionDate,
        InvestmentTransactionType transactionType,
        decimal quantity,
        decimal unitPrice,
        decimal fees,
        string currency,
        string? reference = null)
        : base(id, tenantId)
    {
        if (investmentAccountId == Guid.Empty) throw new ArgumentException("Investment account is required.", nameof(investmentAccountId));
        if (securityId == Guid.Empty) throw new ArgumentException("Security is required.", nameof(securityId));
        if (quantity < 0m) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must not be negative.");
        if (unitPrice < 0m) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must not be negative.");
        if (fees < 0m) throw new ArgumentOutOfRangeException(nameof(fees), "Fees must not be negative.");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));

        if ((transactionType is InvestmentTransactionType.Buy or InvestmentTransactionType.Sell) && quantity <= 0m)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Buy and sell transactions require a positive quantity.");

        InvestmentAccountId = investmentAccountId;
        SecurityId = securityId;
        TransactionDate = transactionDate.Date;
        TransactionType = transactionType;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Fees = fees;
        Currency = currency.Trim().ToUpperInvariant();
        Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
    }

    public Guid InvestmentAccountId { get; private set; }
    public Guid SecurityId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public InvestmentTransactionType TransactionType { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Fees { get; private set; }
    public string Currency { get; private set; } = null!;
    public string? Reference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public decimal GrossAmount => decimal.Round(Quantity * UnitPrice, 4, MidpointRounding.ToEven);
}
