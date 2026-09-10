using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Liabilities;

public enum LiabilityType { Mortgage, PersonalLoan, CreditCard, EducationLoan, Other }

public sealed class Liability : TenantEntity<Guid>, IAuditableEntity
{
    private Liability() : base(Guid.NewGuid(), Guid.NewGuid()) { }
    public Liability(Guid id, Guid tenantId, string name, LiabilityType type, string currency, decimal principal, decimal annualInterestRate, DateTime startDate, DateTime? maturityDate = null) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name)); if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency)); if (principal < 0) throw new ArgumentOutOfRangeException(nameof(principal)); if (annualInterestRate < 0) throw new ArgumentOutOfRangeException(nameof(annualInterestRate)); if (startDate == default) throw new ArgumentException("Start date is required.", nameof(startDate)); if (maturityDate is not null && maturityDate.Value.Date < startDate.Date) throw new ArgumentException("Maturity cannot precede start date.", nameof(maturityDate));
        Name = name.Trim(); Type = type; Currency = currency.Trim().ToUpperInvariant(); OriginalPrincipal = principal; OutstandingPrincipal = principal; AnnualInterestRate = annualInterestRate; StartDate = startDate.Date; MaturityDate = maturityDate?.Date; IsActive = true;
    }
    public string Name { get; private set; } = null!; public LiabilityType Type { get; private set; } public string Currency { get; private set; } = null!; public decimal OriginalPrincipal { get; private set; } public decimal OutstandingPrincipal { get; private set; } public decimal AnnualInterestRate { get; private set; } public DateTime StartDate { get; private set; } public DateTime? MaturityDate { get; private set; } public bool IsActive { get; private set; } public DateTime CreatedAt { get; private set; } public DateTime? UpdatedAt { get; private set; }
    public void ApplyRepayment(decimal principalAmount) { if (principalAmount <= 0) throw new ArgumentOutOfRangeException(nameof(principalAmount)); OutstandingPrincipal = Math.Max(0, OutstandingPrincipal - principalAmount); if (OutstandingPrincipal == 0) IsActive = false; }
}

public sealed class LiabilityRepayment : TenantEntity<Guid>, IAuditableEntity
{
    private LiabilityRepayment() : base(Guid.NewGuid(), Guid.NewGuid()) { }
    public LiabilityRepayment(Guid id, Guid tenantId, Guid liabilityId, DateTime paymentDate, decimal amount, decimal principalAmount, decimal interestAmount) : base(id, tenantId)
    {
        if (liabilityId == Guid.Empty) throw new ArgumentException("LiabilityId is required.", nameof(liabilityId)); if (paymentDate == default) throw new ArgumentException("Payment date is required.", nameof(paymentDate)); if (amount <= 0 || principalAmount < 0 || interestAmount < 0 || principalAmount + interestAmount > amount) throw new ArgumentOutOfRangeException(nameof(amount), "Invalid repayment split.");
        LiabilityId = liabilityId; PaymentDate = paymentDate.Date; Amount = amount; PrincipalAmount = principalAmount; InterestAmount = interestAmount;
    }
    public Guid LiabilityId { get; private set; } public DateTime PaymentDate { get; private set; } public decimal Amount { get; private set; } public decimal PrincipalAmount { get; private set; } public decimal InterestAmount { get; private set; } public DateTime CreatedAt { get; private set; } public DateTime? UpdatedAt { get; private set; }
}
