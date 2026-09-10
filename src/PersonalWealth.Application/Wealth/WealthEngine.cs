namespace PersonalWealth.Application.Wealth;

public sealed record WealthSnapshot(decimal TotalAssets, decimal TotalLiabilities, decimal NetWorth, decimal CashFlow, decimal SavingsRate, decimal DebtToAssetRatio, decimal LiquidityRatio, DateTime AsOfDate);
public sealed record WealthInputs(decimal BankBalance, decimal InvestmentValue, decimal AssetValue, decimal LiabilityBalance, decimal Income, decimal Expenses, decimal LiquidAssets, DateTime AsOfDate);
public interface IWealthEngine { WealthSnapshot Calculate(WealthInputs input); }

public sealed class WealthEngine : IWealthEngine
{
    public WealthSnapshot Calculate(WealthInputs input)
    {
        if (input.AsOfDate == default) throw new ArgumentException("As-of date is required.", nameof(input)); if (input.Income < 0 || input.Expenses < 0 || input.LiabilityBalance < 0) throw new ArgumentOutOfRangeException(nameof(input));
        var assets = input.BankBalance + input.InvestmentValue + input.AssetValue; var netWorth = assets - input.LiabilityBalance; var cashFlow = input.Income - input.Expenses;
        var savingsRate = input.Income == 0 ? 0 : cashFlow / input.Income * 100m; var debtRatio = assets == 0 ? 0 : input.LiabilityBalance / assets * 100m; var liquidity = input.LiabilityBalance == 0 ? 0 : input.LiquidAssets / input.LiabilityBalance;
        return new WealthSnapshot(R(assets), R(input.LiabilityBalance), R(netWorth), R(cashFlow), R(savingsRate), R(debtRatio), R(liquidity), input.AsOfDate.Date);
    }
    private static decimal R(decimal value) => decimal.Round(value, 4, MidpointRounding.ToEven);
}
