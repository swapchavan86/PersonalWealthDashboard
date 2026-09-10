namespace PersonalWealth.Contracts.Wealth;
public sealed record WealthDashboardResponse(decimal Cash,decimal Investments,decimal OtherAssets,decimal Liabilities,decimal NetWorth);
