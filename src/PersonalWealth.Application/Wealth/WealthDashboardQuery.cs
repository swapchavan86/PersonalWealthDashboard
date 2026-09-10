namespace PersonalWealth.Application.Wealth;
public sealed record WealthDashboardDto(decimal Cash, decimal Investments, decimal OtherAssets, decimal Liabilities, decimal NetWorth);
public interface IWealthDashboardQuery { Task<WealthDashboardDto> GetAsync(CancellationToken cancellationToken = default); }
