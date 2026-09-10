using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Expenses;

public sealed class Expense : TenantEntity<Guid>
{
    private Expense()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public Expense(Guid id, Guid tenantId, Guid categoryId, DateTime expenseDate, decimal amount, string description)
        : base(id, tenantId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId must not be empty.", nameof(categoryId));
        if (expenseDate == default) throw new ArgumentException("Expense date is required.", nameof(expenseDate));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));

        CategoryId = categoryId;
        ExpenseDate = expenseDate.Date;
        Amount = amount;
        Description = description.Trim();
    }

    public Guid CategoryId { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = null!;
    public Guid? BankTransactionId { get; private set; }

    public void Update(Guid categoryId, DateTime expenseDate, decimal amount, string description)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId must not be empty.", nameof(categoryId));
        if (expenseDate == default) throw new ArgumentException("Expense date is required.", nameof(expenseDate));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));

        CategoryId = categoryId;
        ExpenseDate = expenseDate.Date;
        Amount = amount;
        Description = description.Trim();
    }

    public void LinkBankTransaction(Guid bankTransactionId)
    {
        if (bankTransactionId == Guid.Empty)
            throw new ArgumentException("BankTransactionId must not be empty.", nameof(bankTransactionId));

        BankTransactionId = bankTransactionId;
    }
}
