using PersonalWealth.Domain.Liabilities;
namespace PersonalWealth.Application.Liabilities;
public sealed record LiabilitySnapshot(Guid LiabilityId,DateTime AsOfDate,decimal Outstanding);
public static class LiabilityAnalytics
{
 public static IReadOnlyList<LiabilitySnapshot> History(IEnumerable<LiabilitySnapshot> snapshots)=>snapshots.OrderBy(x=>x.AsOfDate).ThenBy(x=>x.LiabilityId).ToList();
 public static decimal DebtToAsset(decimal liabilities,decimal assets)=>assets==0?0:decimal.Round(liabilities/assets*100m,4,MidpointRounding.ToEven);
 public static decimal Liquidity(decimal liquidAssets,decimal liabilities)=>liabilities==0?0:decimal.Round(liquidAssets/liabilities,4,MidpointRounding.ToEven);
}
