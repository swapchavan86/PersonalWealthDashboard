using PersonalWealth.Domain.Expenses;
using Xunit;

namespace PersonalWealth.UnitTests.Expenses;

public sealed class ExpenseDomainTests
{
    [Fact]
    public void Expense_rejects_non_positive_amount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Expense(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 9, 10), 0m, "Lunch"));
    }

    [Fact]
    public void Category_rejects_self_parent()
    {
        var id = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            new ExpenseCategory(id, Guid.NewGuid(), "Food", id));
    }

    [Fact]
    public void Expense_normalizes_date_and_description()
    {
        var expense = new Expense(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 9, 10, 15, 20, 0), 125.50m, "  Lunch  ");

        Assert.Equal(new DateTime(2026, 9, 10), expense.ExpenseDate);
        Assert.Equal("Lunch", expense.Description);
    }
}
