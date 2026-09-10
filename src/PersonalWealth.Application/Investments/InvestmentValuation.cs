using PersonalWealth.Domain.Investments;

namespace PersonalWealth.Application.Investments;

public sealed record MarketPrice(Guid SecurityId, DateTime PriceDate, decimal UnitPrice, string Currency, string Source);

public interface IMarketPriceProvider
{
    Task<IReadOnlyCollection<MarketPrice>> GetPricesAsync(IReadOnlyCollection<Guid> securityIds, DateTime asOfDate, CancellationToken cancellationToken = default);
}

public sealed record InvestmentPosition(
    Guid SecurityId,
    decimal Quantity,
    decimal CostBasis,
    decimal MarketPrice,
    decimal MarketValue,
    decimal UnrealizedGainLoss,
    decimal UnrealizedGainLossPercent);

public sealed record InvestmentValuation(
    DateTime AsOfDate,
    decimal TotalCostBasis,
    decimal TotalMarketValue,
    decimal TotalUnrealizedGainLoss,
    IReadOnlyCollection<InvestmentPosition> Positions);

public interface IInvestmentValuationService
{
    InvestmentValuation Calculate(
        IReadOnlyCollection<InvestmentHolding> holdings,
        IReadOnlyCollection<MarketPrice> prices,
        DateTime asOfDate);
}

public sealed class InvestmentValuationService : IInvestmentValuationService
{
    public InvestmentValuation Calculate(
        IReadOnlyCollection<InvestmentHolding> holdings,
        IReadOnlyCollection<MarketPrice> prices,
        DateTime asOfDate)
    {
        ArgumentNullException.ThrowIfNull(holdings);
        ArgumentNullException.ThrowIfNull(prices);

        var priceMap = prices
            .GroupBy(x => x.SecurityId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(x => x.PriceDate.Date).First());

        var positions = holdings
            .Where(x => x.Quantity > 0m)
            .OrderBy(x => x.SecurityId)
            .Select(holding =>
            {
                if (!priceMap.TryGetValue(holding.SecurityId, out var price))
                    throw new InvalidOperationException($"A market price is required for security {holding.SecurityId}.");
                if (price.UnitPrice < 0m)
                    throw new InvalidOperationException($"Market price must not be negative for security {holding.SecurityId}.");

                var marketValue = Round(holding.Quantity * price.UnitPrice);
                var gainLoss = Round(marketValue - holding.CostBasis);
                var gainLossPercent = holding.CostBasis == 0m
                    ? 0m
                    : Round((gainLoss / holding.CostBasis) * 100m);

                return new InvestmentPosition(
                    holding.SecurityId,
                    holding.Quantity,
                    holding.CostBasis,
                    price.UnitPrice,
                    marketValue,
                    gainLoss,
                    gainLossPercent);
            })
            .ToArray();

        var totalCost = Round(positions.Sum(x => x.CostBasis));
        var totalMarketValue = Round(positions.Sum(x => x.MarketValue));
        var totalGainLoss = Round(totalMarketValue - totalCost);

        return new InvestmentValuation(asOfDate.Date, totalCost, totalMarketValue, totalGainLoss, positions);
    }

    private static decimal Round(decimal value) => decimal.Round(value, 4, MidpointRounding.ToEven);
}
