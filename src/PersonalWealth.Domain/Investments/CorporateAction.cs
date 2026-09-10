using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Investments;

public enum CorporateActionType
{
    StockSplit,
    BonusIssue,
    SecurityRename,
    SymbolChange
}

public sealed class CorporateAction : TenantEntity<Guid>, IAuditableEntity
{
    private CorporateAction() : base(Guid.NewGuid(), Guid.NewGuid()) { }

    public CorporateAction(Guid id, Guid tenantId, Guid securityId, CorporateActionType actionType, DateTime effectiveDate, decimal ratioNumerator, decimal ratioDenominator, string? newSymbol = null, string? newName = null)
        : base(id, tenantId)
    {
        if (securityId == Guid.Empty) throw new ArgumentException("SecurityId is required.", nameof(securityId));
        if (effectiveDate == default) throw new ArgumentException("Effective date is required.", nameof(effectiveDate));
        if (ratioNumerator <= 0 || ratioDenominator <= 0) throw new ArgumentOutOfRangeException(nameof(ratioNumerator), "Corporate-action ratios must be positive.");
        if (actionType == CorporateActionType.SymbolChange && string.IsNullOrWhiteSpace(newSymbol)) throw new ArgumentException("New symbol is required for symbol changes.", nameof(newSymbol));
        if (actionType == CorporateActionType.SecurityRename && string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("New name is required for renames.", nameof(newName));
        SecurityId = securityId; ActionType = actionType; EffectiveDate = effectiveDate.Date;
        RatioNumerator = ratioNumerator; RatioDenominator = ratioDenominator;
        NewSymbol = newSymbol?.Trim().ToUpperInvariant(); NewName = newName?.Trim();
    }

    public Guid SecurityId { get; private set; }
    public CorporateActionType ActionType { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public decimal RatioNumerator { get; private set; }
    public decimal RatioDenominator { get; private set; }
    public string? NewSymbol { get; private set; }
    public string? NewName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public decimal QuantityMultiplier => RatioNumerator / RatioDenominator;
}
