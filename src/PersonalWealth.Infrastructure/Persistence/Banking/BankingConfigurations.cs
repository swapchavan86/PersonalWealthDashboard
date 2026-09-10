using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalWealth.Domain.Banking;

namespace PersonalWealth.Infrastructure.Persistence.Banking;

public sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Institution).HasMaxLength(200).IsRequired();
        builder.Property(x => x.AccountNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.AccountType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.TenantId, x.AccountNumber }).IsUnique();
    }
}

public sealed class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.ToTable("BankTransactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Direction).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.SourceFingerprint).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.HasIndex(x => new { x.TenantId, x.AccountId, x.SourceFingerprint }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ImportId });
        builder.HasOne<BankAccount>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
