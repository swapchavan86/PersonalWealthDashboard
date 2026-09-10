using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalWealth.Domain.Expenses;

namespace PersonalWealth.Infrastructure.Persistence.Configurations;

public sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.Name });
        builder.HasIndex(x => new { x.TenantId, x.ParentCategoryId });
    }
}

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(19, 4);
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.ExpenseDate });
        builder.HasIndex(x => new { x.TenantId, x.CategoryId, x.ExpenseDate });
        builder.HasIndex(x => new { x.TenantId, x.BankTransactionId }).IsUnique().HasFilter("[BankTransactionId] IS NOT NULL");
    }
}

public sealed class RecurringExpenseConfiguration : IEntityTypeConfiguration<RecurringExpense>
{
    public void Configure(EntityTypeBuilder<RecurringExpense> builder)
    {
        builder.ToTable("RecurringExpenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(19, 4);
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Frequency).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.TenantId, x.StartDate });
        builder.HasIndex(x => new { x.TenantId, x.IsActive });
    }
}
