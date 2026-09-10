using PersonalWealth.Domain.Investments;

namespace PersonalWealth.Application.Investments;

public sealed record PortfolioReplayResult(
    IReadOnlyCollection<InvestmentHolding> Holdings,
    decimal RealizedGainLoss,
    decimal DividendIncome,
    decimal Fees);

public interface IInvestmentPortfolioEngine
{
    PortfolioReplayResult Replay(
        Guid tenantId,
        IReadOnlyCollection<InvestmentTransaction> transactions);
}

public sealed class InvestmentPortfolioEngine : IInvestmentPortfolioEngine
{
    public PortfolioReplayResult Replay(
        Guid tenantId,
        IReadOnlyCollection<InvestmentTransaction> transactions)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId must not be empty.", nameof(tenantId));
        ArgumentNullException.ThrowIfNull(transactions);

        var ordered = transactions
            .OrderBy(x => x.TransactionDate)
            .ThenBy(x => x.Id)
            .ToArray();

        var holdings = new Dictionary<(Guid AccountId, Guid SecurityId), InvestmentHolding>();
        decimal realizedGainLoss = 0m;
        decimal dividendIncome = 0m;
        decimal fees = 0m;

        foreach (var transaction in ordered)
        {
            if (transaction.TenantId != tenantId)
                throw new InvalidOperationException("All investment transactions must belong to the requested tenant.");

            var key = (transaction.InvestmentAccountId, transaction.SecurityId);
            if (!holdings.TryGetValue(key, out var holding))
            {
                holding = new InvestmentHolding(Guid.NewGuid(), tenantId, transaction.InvestmentAccountId, transaction.SecurityId);
                holdings[key] = holding;
            }

            fees += transaction.Fees;

            switch (transaction.TransactionType)
            {
                case InvestmentTransactionType.Buy:
                    holding.ApplyBuy(transaction.Quantity, transaction.UnitPrice, transaction.Fees);
                    break;
                case InvestmentTransactionType.Sell:
                    realizedGainLoss += holding.ApplySell(transaction.Quantity, transaction.UnitPrice, transaction.Fees);
                    break;
                case InvestmentTransactionType.Dividend:
                    dividendIncome += transaction.GrossAmount - transaction.Fees;
                    break;
                case InvestmentTransactionType.Fee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transaction), transaction.TransactionType, "Unsupported investment transaction type.");
            }
        }

        return new PortfolioReplayResult(
            holdings.Values.OrderBy(x => x.InvestmentAccountId).ThenBy(x => x.SecurityId).ToArray(),
            Round(realizedGainLoss),
            Round(dividendIncome),
            Round(fees));
    }

    private static decimal Round(decimal value) => decimal.Round(value, 4, MidpointRounding.ToEven);
}
