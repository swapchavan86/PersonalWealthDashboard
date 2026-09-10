using PersonalWealth.Domain.Investments;

namespace PersonalWealth.Application.Investments;

public sealed record InvestmentReconciliationLine(Guid SecurityId, decimal ExpectedQuantity, decimal ActualQuantity, decimal QuantityVariance, decimal ExpectedCostBasis, decimal ActualCostBasis, decimal CostVariance);
public sealed record InvestmentReconciliationResult(IReadOnlyList<InvestmentReconciliationLine> Lines, bool IsBalanced)
{
    public decimal TotalQuantityVariance => Lines.Sum(x => Math.Abs(x.QuantityVariance));
    public decimal TotalCostVariance => Lines.Sum(x => Math.Abs(x.CostVariance));
}

public interface IInvestmentReconciliationService
{
    InvestmentReconciliationResult Reconcile(IEnumerable<InvestmentHolding> expected, IEnumerable<InvestmentHolding> actual, decimal quantityTolerance = 0.0001m, decimal costTolerance = 0.01m);
}

public sealed class InvestmentReconciliationService : IInvestmentReconciliationService
{
    public InvestmentReconciliationResult Reconcile(IEnumerable<InvestmentHolding> expected, IEnumerable<InvestmentHolding> actual, decimal quantityTolerance = 0.0001m, decimal costTolerance = 0.01m)
    {
        if (quantityTolerance < 0 || costTolerance < 0) throw new ArgumentOutOfRangeException();
        var e = expected.ToDictionary(x => (x.InvestmentAccountId, x.SecurityId));
        var a = actual.ToDictionary(x => (x.InvestmentAccountId, x.SecurityId));
        var keys = e.Keys.Union(a.Keys).OrderBy(x => x.Item1).ThenBy(x => x.Item2);
        var lines = keys.Select(key =>
        {
            e.TryGetValue(key, out var eh); a.TryGetValue(key, out var ah);
            var eq = eh?.Quantity ?? 0m; var aq = ah?.Quantity ?? 0m;
            var ec = eh?.CostBasis ?? 0m; var ac = ah?.CostBasis ?? 0m;
            return new InvestmentReconciliationLine(key.SecurityId, eq, aq, aq - eq, ec, ac, ac - ec);
        }).ToList();
        return new InvestmentReconciliationResult(lines, lines.All(x => Math.Abs(x.QuantityVariance) <= quantityTolerance && Math.Abs(x.CostVariance) <= costTolerance));
    }
}
