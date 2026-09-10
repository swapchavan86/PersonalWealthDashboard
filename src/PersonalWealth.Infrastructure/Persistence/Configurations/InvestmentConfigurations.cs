using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalWealth.Domain.Investments;

namespace PersonalWealth.Infrastructure.Persistence.Configurations;

public sealed class InvestmentAccountConfiguration : IEntityTypeConfiguration<InvestmentAccount>
{
    public void Configure(EntityTypeBuilder<InvestmentAccount> builder)
    {
        builder.ToTable("InvestmentAccounts"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Institution).HasMaxLength(200).IsRequired(); builder.Property(x => x.AccountNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.AccountType).HasConversion<string>().HasMaxLength(30); builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20); builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.AccountNumber }).IsUnique();
    }
}
public sealed class SecurityConfiguration : IEntityTypeConfiguration<Security>
{
    public void Configure(EntityTypeBuilder<Security> builder)
    {
        builder.ToTable("Securities"); builder.HasKey(x => x.Id); builder.Property(x => x.Symbol).HasMaxLength(50).IsRequired(); builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.SecurityType).HasConversion<string>().HasMaxLength(30); builder.Property(x => x.Currency).HasMaxLength(3).IsRequired(); builder.Property(x => x.Isin).HasMaxLength(20);
        builder.HasIndex(x => new { x.TenantId, x.Symbol }).IsUnique(); builder.HasIndex(x => new { x.TenantId, x.Isin });
    }
}
public sealed class InvestmentHoldingConfiguration : IEntityTypeConfiguration<InvestmentHolding>
{
    public void Configure(EntityTypeBuilder<InvestmentHolding> builder)
    {
        builder.ToTable("InvestmentHoldings"); builder.HasKey(x => x.Id); builder.Property(x => x.Quantity).HasPrecision(28, 10); builder.Property(x => x.CostBasis).HasPrecision(19, 4);
        builder.HasIndex(x => new { x.TenantId, x.InvestmentAccountId, x.SecurityId }).IsUnique(); builder.HasOne<InvestmentAccount>().WithMany().HasForeignKey(x => x.InvestmentAccountId).OnDelete(DeleteBehavior.Restrict); builder.HasOne<Security>().WithMany().HasForeignKey(x => x.SecurityId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class InvestmentTransactionConfiguration : IEntityTypeConfiguration<InvestmentTransaction>
{
    public void Configure(EntityTypeBuilder<InvestmentTransaction> builder)
    {
        builder.ToTable("InvestmentTransactions"); builder.HasKey(x => x.Id); builder.Property(x => x.TransactionType).HasConversion<string>().HasMaxLength(20); builder.Property(x => x.Quantity).HasPrecision(28, 10); builder.Property(x => x.UnitPrice).HasPrecision(19, 4); builder.Property(x => x.Fees).HasPrecision(19, 4); builder.Property(x => x.Currency).HasMaxLength(3).IsRequired(); builder.Property(x => x.Reference).HasMaxLength(200);
        builder.HasIndex(x => new { x.TenantId, x.InvestmentAccountId, x.TransactionDate }); builder.HasIndex(x => new { x.TenantId, x.SecurityId, x.TransactionDate }); builder.HasOne<InvestmentAccount>().WithMany().HasForeignKey(x => x.InvestmentAccountId).OnDelete(DeleteBehavior.Restrict); builder.HasOne<Security>().WithMany().HasForeignKey(x => x.SecurityId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class CorporateActionConfiguration : IEntityTypeConfiguration<CorporateAction>
{
    public void Configure(EntityTypeBuilder<CorporateAction> builder)
    {
        builder.ToTable("CorporateActions"); builder.HasKey(x => x.Id); builder.Property(x => x.ActionType).HasConversion<string>().HasMaxLength(30); builder.Property(x => x.RatioNumerator).HasPrecision(19, 10); builder.Property(x => x.RatioDenominator).HasPrecision(19, 10); builder.Property(x => x.NewSymbol).HasMaxLength(50); builder.Property(x => x.NewName).HasMaxLength(250);
        builder.HasIndex(x => new { x.TenantId, x.SecurityId, x.EffectiveDate }); builder.HasOne<Security>().WithMany().HasForeignKey(x => x.SecurityId).OnDelete(DeleteBehavior.Restrict);
    }
}
