using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Expenses;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Domain.Expenses;
using PersonalWealth.Infrastructure.Persistence;

namespace PersonalWealth.Infrastructure.Expenses;

public sealed class EfExpenseCategoryRepository(PersonalWealthDbContext dbContext) : IExpenseCategoryRepository
{
    public async Task AddAsync(ExpenseCategoryModel category, CancellationToken cancellationToken = default)
    {
        dbContext.Add(new ExpenseCategory(category.Id, category.TenantId, category.Name, category.ParentCategoryId));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ExpenseCategoryModel?> GetAsync(Guid tenantId, Guid categoryId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<ExpenseCategory>()
            .Where(x => x.TenantId == tenantId && x.Id == categoryId)
            .Select(x => new ExpenseCategoryModel(x.Id, x.TenantId, x.Name, x.ParentCategoryId, x.IsActive))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyCollection<ExpenseCategoryModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<ExpenseCategory>()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .Select(x => new ExpenseCategoryModel(x.Id, x.TenantId, x.Name, x.ParentCategoryId, x.IsActive))
            .ToListAsync(cancellationToken);
}

public sealed class EfExpenseRepository(PersonalWealthDbContext dbContext) : IExpenseRepository
{
    public async Task AddAsync(ExpenseModel expense, CancellationToken cancellationToken = default)
    {
        var entity = new Expense(expense.Id, expense.TenantId, expense.CategoryId, expense.ExpenseDate, expense.Amount, expense.Description);
        if (expense.BankTransactionId.HasValue) entity.LinkBankTransaction(expense.BankTransactionId.Value);
        dbContext.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ExpenseModel?> GetAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<Expense>()
            .Where(x => x.TenantId == tenantId && x.Id == expenseId)
            .Select(MapExpression())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task UpdateAsync(ExpenseModel expense, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Set<Expense>().SingleOrDefaultAsync(x => x.TenantId == expense.TenantId && x.Id == expense.Id, cancellationToken)
            ?? throw new InvalidOperationException("Expense was not found.");
        entity.Update(expense.CategoryId, expense.ExpenseDate, expense.Amount, expense.Description);
        if (expense.BankTransactionId.HasValue) entity.LinkBankTransaction(expense.BankTransactionId.Value);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ExpenseModel>> ListAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<Expense>().Where(x => x.TenantId == tenantId);
        if (from.HasValue) query = query.Where(x => x.ExpenseDate >= from.Value.Date);
        if (to.HasValue) query = query.Where(x => x.ExpenseDate <= to.Value.Date);
        return await query.OrderBy(x => x.ExpenseDate).Select(MapExpression()).ToListAsync(cancellationToken);
    }

    public async Task LinkBankTransactionAsync(Guid tenantId, Guid expenseId, Guid bankTransactionId, CancellationToken cancellationToken = default)
    {
        var transactionExists = await dbContext.Set<BankTransaction>().AnyAsync(x => x.TenantId == tenantId && x.Id == bankTransactionId, cancellationToken);
        if (!transactionExists) throw new KeyNotFoundException("Bank transaction was not found for the tenant.");

        var alreadyLinked = await dbContext.Set<Expense>().AnyAsync(x => x.TenantId == tenantId && x.BankTransactionId == bankTransactionId && x.Id != expenseId, cancellationToken);
        if (alreadyLinked) throw new InvalidOperationException("Bank transaction is already linked to another expense.");

        var entity = await dbContext.Set<Expense>().SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == expenseId, cancellationToken)
            ?? throw new KeyNotFoundException("Expense was not found.");
        entity.LinkBankTransaction(bankTransactionId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static System.Linq.Expressions.Expression<Func<Expense, ExpenseModel>> MapExpression() =>
        x => new ExpenseModel(x.Id, x.TenantId, x.CategoryId, x.ExpenseDate, x.Amount, x.Description, x.BankTransactionId);
}

public sealed class EfRecurringExpenseRepository(PersonalWealthDbContext dbContext) : IRecurringExpenseRepository
{
    public async Task AddAsync(RecurringExpenseModel recurringExpense, CancellationToken cancellationToken = default)
    {
        dbContext.Add(new RecurringExpense(recurringExpense.Id, recurringExpense.TenantId, recurringExpense.CategoryId, recurringExpense.StartDate, recurringExpense.Amount, recurringExpense.Description, recurringExpense.Frequency, recurringExpense.Interval, recurringExpense.EndDate));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RecurringExpenseModel?> GetAsync(Guid tenantId, Guid recurringExpenseId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<RecurringExpense>()
            .Where(x => x.TenantId == tenantId && x.Id == recurringExpenseId)
            .Select(x => new RecurringExpenseModel(x.Id, x.TenantId, x.CategoryId, x.StartDate, x.Amount, x.Description, x.Frequency, x.Interval, x.EndDate, x.IsActive))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyCollection<RecurringExpenseModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<RecurringExpense>()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.StartDate)
            .Select(x => new RecurringExpenseModel(x.Id, x.TenantId, x.CategoryId, x.StartDate, x.Amount, x.Description, x.Frequency, x.Interval, x.EndDate, x.IsActive))
            .ToListAsync(cancellationToken);
}

public sealed class EfExpenseReportingRepository(PersonalWealthDbContext dbContext) : IExpenseReportingRepository
{
    public async Task<ExpenseReportModel> GetReportAsync(Guid tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var expenses = await dbContext.Set<Expense>()
            .Where(x => x.TenantId == tenantId && x.ExpenseDate >= from && x.ExpenseDate <= to)
            .Select(x => new { x.CategoryId, x.ExpenseDate, x.Amount })
            .ToListAsync(cancellationToken);

        var byCategory = expenses
            .GroupBy(x => x.CategoryId)
            .OrderBy(x => x.Key)
            .Select(x => new ExpenseCategoryTotal(x.Key, x.Sum(y => y.Amount), x.Count()))
            .ToArray();

        var byMonth = expenses
            .GroupBy(x => new DateTime(x.ExpenseDate.Year, x.ExpenseDate.Month, 1))
            .OrderBy(x => x.Key)
            .Select(x => new ExpenseMonthlyTotal(x.Key, x.Sum(y => y.Amount), x.Count()))
            .ToArray();

        var trend = byMonth.Select(x => new ExpenseTrendPoint(x.Month, x.TotalAmount)).ToArray();
        return new ExpenseReportModel(expenses.Sum(x => x.Amount), expenses.Count, byCategory, byMonth, trend);
    }
}
