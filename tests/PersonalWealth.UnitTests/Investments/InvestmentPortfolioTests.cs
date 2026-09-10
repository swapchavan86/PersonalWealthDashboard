using PersonalWealth.Application.Investments;
using PersonalWealth.Domain.Investments;

namespace PersonalWealth.UnitTests.Investments;

public sealed class InvestmentPortfolioTests
{
    [Fact]
    public void Buy_then_sell_replays_weighted_average_cost_and_realized_gain()
    {
        var tenantId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var securityId = Guid.NewGuid();
        var transactions = new[]
        {
            new InvestmentTransaction(Guid.NewGuid(), tenantId, accountId, securityId, new DateTime(2026, 1, 1), InvestmentTransactionType.Buy, 10m, 100m, 10m, "INR"),
            new InvestmentTransaction(Guid.NewGuid(), tenantId, accountId, securityId, new DateTime(2026, 1, 2), InvestmentTransactionType.Buy, 10m, 120m, 10m, "INR"),
            new InvestmentTransaction(Guid.NewGuid(), tenantId, accountId, securityId, new DateTime(2026, 1, 3), InvestmentTransactionType.Sell, 10m, 150m, 5m, "INR")
        };

        var result = new InvestmentPortfolioEngine().Replay(tenantId, transactions);

        Assert.Single(result.Holdings);
        Assert.Equal(10m, result.Holdings.Single().Quantity);
        Assert.Equal(111m, result.Holdings.Single().AverageCost);
        Assert.Equal(385m, result.RealizedGainLoss);
        Assert.Equal(25m, result.Fees);
    }

    [Fact]
    public void Replay_is_deterministic_when_transactions_arrive_out_of_order()
    {
        var tenantId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var securityId = Guid.NewGuid();
        var first = new InvestmentTransaction(Guid.NewGuid(), tenantId, accountId, securityId, new DateTime(2026, 2, 2), InvestmentTransactionType.Buy, 5m, 100m, 0m, "INR");
        var second = new InvestmentTransaction(Guid.NewGuid(), tenantId, accountId, securityId, new DateTime(2026, 2, 1), InvestmentTransactionType.Buy, 5m, 80m, 0m, "INR");

        var result = new InvestmentPortfolioEngine().Replay(tenantId, new[] { first, second });

        Assert.Equal(10m, result.Holdings.Single().Quantity);
        Assert.Equal(90m, result.Holdings.Single().AverageCost);
    }

    [Fact]
    public void Sell_cannot_exceed_current_holding()
    {
        var holding = new InvestmentHolding(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        holding.ApplyBuy(2m, 100m, 0m);

        Assert.Throws<InvalidOperationException>(() => holding.ApplySell(3m, 120m, 0m));
    }

    [Fact]
    public void Valuation_calculates_market_value_and_unrealized_gain_loss()
    {
        var tenantId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var securityId = Guid.NewGuid();
        var holding = new InvestmentHolding(Guid.NewGuid(), tenantId, accountId, securityId);
        holding.ApplyBuy(10m, 100m, 10m);

        var valuation = new InvestmentValuationService().Calculate(
            new[] { holding },
            new[] { new MarketPrice(securityId, new DateTime(2026, 3, 1), 125m, "INR", "test") },
            new DateTime(2026, 3, 1));

        Assert.Equal(1010m, valuation.TotalCostBasis);
        Assert.Equal(1250m, valuation.TotalMarketValue);
        Assert.Equal(240m, valuation.TotalUnrealizedGainLoss);
        Assert.Equal(1, valuation.Positions.Count);
        Assert.Equal(125m, valuation.Positions.Single().MarketPrice);
    }

    [Fact]
    public void Valuation_requires_a_price_for_each_open_position()
    {
        var tenantId = Guid.NewGuid();
        var holding = new InvestmentHolding(Guid.NewGuid(), tenantId, Guid.NewGuid(), Guid.NewGuid());
        holding.ApplyBuy(1m, 100m, 0m);

        Assert.Throws<InvalidOperationException>(() => new InvestmentValuationService().Calculate(
            new[] { holding },
            Array.Empty<MarketPrice>(),
            new DateTime(2026, 3, 1)));
    }
}
