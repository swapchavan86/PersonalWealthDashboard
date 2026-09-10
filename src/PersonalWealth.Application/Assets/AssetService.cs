using PersonalWealth.Domain.Assets;

namespace PersonalWealth.Application.Assets;

public sealed record AssetSnapshot(Guid AssetId, string Name, AssetType Type, string Currency, decimal Value, DateTime AsOfDate, string Source);
public interface IAssetValuationService { AssetSnapshot GetLatest(Asset asset, IEnumerable<AssetValuation> valuations); }

public sealed class AssetValuationService : IAssetValuationService
{
    public AssetSnapshot GetLatest(Asset asset, IEnumerable<AssetValuation> valuations)
    {
        var latest = valuations.Where(x => x.AssetId == asset.Id).OrderByDescending(x => x.ValuationDate).ThenByDescending(x => x.Id).FirstOrDefault();
        return latest is null ? new AssetSnapshot(asset.Id, asset.Name, asset.Type, asset.Currency, asset.AcquisitionValue, asset.AcquisitionDate, "ACQUISITION") : new AssetSnapshot(asset.Id, asset.Name, asset.Type, asset.Currency, latest.Value, latest.ValuationDate, latest.Source);
    }
}
