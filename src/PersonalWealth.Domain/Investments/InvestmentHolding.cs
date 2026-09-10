using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Investments;

public sealed class InvestmentHolding : TenantEntity<Guid>, IAuditableEntity
{
    private InvestmentHolding()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public InvestmentHolding(Guid id, Guid tenantId, Guid investmentAccountId, Guid securityId)
        : base(id, tenantId)
    {
        if (investmentAccountId == Guid.Empty) throw new ArgumentException("Investment account is required.", nameof(investmentAccountId));
        if (securityId == Guid.Empty) throw new ArgumentException("Security is required.", nameof(securityId));

        InvestmentAccountId = investmentAccountId;
        SecurityId = securityId;
    }

    public Guid InvestmentAccountId { get; private set; }
    public Guid SecurityId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal CostBasis { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public decimal AverageCost => Quantity == 0m ? 0m : decimal.Round(CostBasis / Quantity, 4, MidpointRounding.ToEven);

    public void ApplyBuy(decimal quantity, decimal unitPrice, decimal fees)
    {
        ValidateTrade(quantity, unitPrice, fees);
        Quantity += quantity;
        CostBasis = decimal.Round(CostBasis + (quantity * unitPrice) + fees, 4, MidpointRounding.ToEven);
    }

    public decimal ApplySell(decimal quantity, decimal unitPrice, decimal fees)
    {
        ValidateTrade(quantity, unitPrice, fees);
        if (quantity > Quantity) throw new InvalidOperationException("Cannot sell more units than the current holding.");

        var averageCost = AverageCost;
        var costOfUnitsSold = decimal.Round(quantity * averageCost, 4, MidpointRounding.ToEven);
        var netProceeds = decimal.Round((quantity * unitPrice) - fees, 4, MidpointRounding.ToEven);

        Quantity -= quantity;
        CostBasis = decimal.Round(CostBasis - costOfUnitsSold, 4, MidpointRounding.ToEven);
        if (Quantity == 0m) CostBasis = 0m;

        return decimal.Round(netProceeds - costOfUnitsSold, 4, MidpointRounding.ToEven);
    }

    private static void ValidateTrade(decimal quantity, decimal unitPrice, decimal fees)
    {
        if (quantity <= 0m) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        if (unitPrice < 0m) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must not be negative.");
        if (fees < 0m) throw new ArgumentOutOfRangeException(nameof(fees), "Fees must not be negative.");
    }
}
