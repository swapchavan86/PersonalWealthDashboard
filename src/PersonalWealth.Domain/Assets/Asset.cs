using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Assets;

public enum AssetType { Property, Vehicle, Cash, PreciousMetal, Business, Other }

public sealed class Asset : TenantEntity<Guid>, IAuditableEntity
{
    private Asset() : base(Guid.NewGuid(), Guid.NewGuid()) { }
    public Asset(Guid id, Guid tenantId, string name, AssetType type, string currency, decimal acquisitionValue, DateTime acquisitionDate) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));
        if (acquisitionValue < 0) throw new ArgumentOutOfRangeException(nameof(acquisitionValue));
        if (acquisitionDate == default) throw new ArgumentException("Acquisition date is required.", nameof(acquisitionDate));
        Name = name.Trim(); Type = type; Currency = currency.Trim().ToUpperInvariant(); AcquisitionValue = acquisitionValue; AcquisitionDate = acquisitionDate.Date; IsActive = true;
    }
    public string Name { get; private set; } = null!; public AssetType Type { get; private set; } public string Currency { get; private set; } = null!;
    public decimal AcquisitionValue { get; private set; } public DateTime AcquisitionDate { get; private set; } public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; } public DateTime? UpdatedAt { get; private set; }
    public void Close() => IsActive = false;
}

public sealed class AssetValuation : TenantEntity<Guid>, IAuditableEntity
{
    private AssetValuation() : base(Guid.NewGuid(), Guid.NewGuid()) { }
    public AssetValuation(Guid id, Guid tenantId, Guid assetId, DateTime valuationDate, decimal value, string source) : base(id, tenantId)
    {
        if (assetId == Guid.Empty) throw new ArgumentException("AssetId is required.", nameof(assetId)); if (valuationDate == default) throw new ArgumentException("Valuation date is required.", nameof(valuationDate)); if (value < 0) throw new ArgumentOutOfRangeException(nameof(value)); if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException("Source is required.", nameof(source));
        AssetId = assetId; ValuationDate = valuationDate.Date; Value = value; Source = source.Trim();
    }
    public Guid AssetId { get; private set; } public DateTime ValuationDate { get; private set; } public decimal Value { get; private set; } public string Source { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } public DateTime? UpdatedAt { get; private set; }
}
