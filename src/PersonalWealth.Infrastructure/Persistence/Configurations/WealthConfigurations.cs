using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalWealth.Domain.Assets;
using PersonalWealth.Domain.Liabilities;

namespace PersonalWealth.Infrastructure.Persistence.Configurations;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> b) { b.ToTable("Assets"); b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.Type).HasConversion<string>().HasMaxLength(30); b.Property(x => x.Currency).HasMaxLength(3).IsRequired(); b.Property(x => x.AcquisitionValue).HasPrecision(19, 4); b.HasIndex(x => new { x.TenantId, x.Name }); }
}
public sealed class AssetValuationConfiguration : IEntityTypeConfiguration<AssetValuation>
{
    public void Configure(EntityTypeBuilder<AssetValuation> b) { b.ToTable("AssetValuations"); b.HasKey(x => x.Id); b.Property(x => x.Value).HasPrecision(19, 4); b.Property(x => x.Source).HasMaxLength(100).IsRequired(); b.HasIndex(x => new { x.TenantId, x.AssetId, x.ValuationDate }); b.HasOne<Asset>().WithMany().HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Cascade); }
}
public sealed class LiabilityConfiguration : IEntityTypeConfiguration<Liability>
{
    public void Configure(EntityTypeBuilder<Liability> b) { b.ToTable("Liabilities"); b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.Type).HasConversion<string>().HasMaxLength(30); b.Property(x => x.Currency).HasMaxLength(3).IsRequired(); b.Property(x => x.OriginalPrincipal).HasPrecision(19, 4); b.Property(x => x.OutstandingPrincipal).HasPrecision(19, 4); b.Property(x => x.AnnualInterestRate).HasPrecision(9, 4); b.HasIndex(x => new { x.TenantId, x.Name }); }
}
public sealed class LiabilityRepaymentConfiguration : IEntityTypeConfiguration<LiabilityRepayment>
{
    public void Configure(EntityTypeBuilder<LiabilityRepayment> b) { b.ToTable("LiabilityRepayments"); b.HasKey(x => x.Id); b.Property(x => x.Amount).HasPrecision(19, 4); b.Property(x => x.PrincipalAmount).HasPrecision(19, 4); b.Property(x => x.InterestAmount).HasPrecision(19, 4); b.HasIndex(x => new { x.TenantId, x.LiabilityId, x.PaymentDate }); b.HasOne<Liability>().WithMany().HasForeignKey(x => x.LiabilityId).OnDelete(DeleteBehavior.Cascade); }
}
