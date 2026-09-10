namespace PersonalWealth.Application.Wealth;

public sealed record WealthComponent(string Category, decimal Value, decimal Percentage);
public sealed record WealthHistoryPoint(DateTime AsOfDate, decimal TotalAssets, decimal TotalLiabilities, decimal NetWorth);

public static class WealthAnalytics
{
    public static IReadOnlyList<WealthComponent> Allocation(IEnumerable<(string Category, decimal Value)> components)
    {
        var values = components.GroupBy(x => x.Category).Select(g => (g.Key, Value: g.Sum(x => x.Value))).OrderByDescending(x => x.Value).ToList(); var total = values.Sum(x => x.Value);
        return values.Select(x => new WealthComponent(x.Key, R(x.Value), R(total == 0 ? 0 : x.Value / total * 100m))).ToList();
    }
    public static decimal Concentration(IEnumerable<(string Category, decimal Value)> components, int topN = 3)
    {
        if (topN <= 0) throw new ArgumentOutOfRangeException(nameof(topN)); var values = components.Select(x => x.Value).OrderByDescending(x => x).ToList(); var total = values.Sum(); return R(total == 0 ? 0 : values.Take(topN).Sum() / total * 100m);
    }
    public static IReadOnlyList<WealthHistoryPoint> History(IEnumerable<WealthHistoryPoint> points) => points.OrderBy(x => x.AsOfDate).ThenBy(x => x.NetWorth).ToList();
    public static decimal GrowthPercent(decimal previous, decimal current) => R(previous == 0 ? 0 : (current - previous) / Math.Abs(previous) * 100m);
    private static decimal R(decimal v) => decimal.Round(v, 4, MidpointRounding.ToEven);
}
