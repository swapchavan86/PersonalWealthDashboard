using PersonalWealth.Application.Results;
using PersonalWealth.Domain.Expenses;

namespace PersonalWealth.Application.Expenses;

public sealed class ExpenseService(
    IExpenseCategoryRepository categoryRepository,
    IExpenseRepository expenseRepository) : IExpenseService
{
    public async Task<Result<ExpenseCategoryModel>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty)
                return Result<ExpenseCategoryModel>.Failure(ApplicationError.Validation("TenantId must not be empty."));

            if (request.Id == Guid.Empty)
                return Result<ExpenseCategoryModel>.Failure(ApplicationError.Validation("Category Id must not be empty."));

            if (request.ParentCategoryId.HasValue)
            {
                var parent = await categoryRepository.GetAsync(request.TenantId, request.ParentCategoryId.Value, cancellationToken);
                if (parent is null)
                    return Result<ExpenseCategoryModel>.Failure(ApplicationError.NotFound("Parent category was not found."));
            }

            var existing = await categoryRepository.GetAsync(request.TenantId, request.Id, cancellationToken);
            if (existing is not null)
                return Result<ExpenseCategoryModel>.Failure(ApplicationError.Conflict("Category already exists."));

            var category = new ExpenseCategory(request.Id, request.TenantId, request.Name, request.ParentCategoryId);
            var model = new ExpenseCategoryModel(category.Id, category.TenantId, category.Name, category.ParentCategoryId, category.IsActive);
            await categoryRepository.AddAsync(model, cancellationToken);
            return Result<ExpenseCategoryModel>.Success(model);
        }
        catch (ArgumentException ex)
        {
            return Result<ExpenseCategoryModel>.Failure(ApplicationError.Validation(ex.Message));
        }
    }

    public async Task<Result<IReadOnlyCollection<ExpenseCategoryModel>>> ListCategoriesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            return Result<IReadOnlyCollection<ExpenseCategoryModel>>.Failure(ApplicationError.Validation("TenantId must not be empty."));

        return Result<IReadOnlyCollection<ExpenseCategoryModel>>.Success(await categoryRepository.ListAsync(tenantId, cancellationToken));
    }

    public async Task<Result<ExpenseModel>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty)
                return Result<ExpenseModel>.Failure(ApplicationError.Validation("TenantId must not be empty."));

            var category = await categoryRepository.GetAsync(request.TenantId, request.CategoryId, cancellationToken);
            if (category is null || !category.IsActive)
                return Result<ExpenseModel>.Failure(ApplicationError.Validation("An active expense category is required."));

            var existing = await expenseRepository.GetAsync(request.TenantId, request.Id, cancellationToken);
            if (existing is not null)
                return Result<ExpenseModel>.Failure(ApplicationError.Conflict("Expense already exists."));

            var expense = new Expense(request.Id, request.TenantId, request.CategoryId, request.ExpenseDate, request.Amount, request.Description);
            var model = Map(expense);
            await expenseRepository.AddAsync(model, cancellationToken);
            return Result<ExpenseModel>.Success(model);
        }
        catch (ArgumentException ex)
        {
            return Result<ExpenseModel>.Failure(ApplicationError.Validation(ex.Message));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Result<ExpenseModel>.Failure(ApplicationError.Validation(ex.Message));
        }
    }

    public async Task<Result<ExpenseModel>> UpdateExpenseAsync(UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty)
                return Result<ExpenseModel>.Failure(ApplicationError.Validation("TenantId must not be empty."));

            var category = await categoryRepository.GetAsync(request.TenantId, request.CategoryId, cancellationToken);
            if (category is null || !category.IsActive)
                return Result<ExpenseModel>.Failure(ApplicationError.Validation("An active expense category is required."));

            var existing = await expenseRepository.GetAsync(request.TenantId, request.ExpenseId, cancellationToken);
            if (existing is null)
                return Result<ExpenseModel>.Failure(ApplicationError.NotFound("Expense was not found."));

            var expense = new Expense(existing.Id, existing.TenantId, existing.CategoryId, existing.ExpenseDate, existing.Amount, existing.Description);
            expense.Update(request.CategoryId, request.ExpenseDate, request.Amount, request.Description);
            var model = Map(expense) with { BankTransactionId = existing.BankTransactionId };
            await expenseRepository.UpdateAsync(model, cancellationToken);
            return Result<ExpenseModel>.Success(model);
        }
        catch (ArgumentException ex)
        {
            return Result<ExpenseModel>.Failure(ApplicationError.Validation(ex.Message));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Result<ExpenseModel>.Failure(ApplicationError.Validation(ex.Message));
        }
    }

    public async Task<Result<ExpenseModel>> GetExpenseAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || expenseId == Guid.Empty)
            return Result<ExpenseModel>.Failure(ApplicationError.Validation("TenantId and ExpenseId are required."));

        var expense = await expenseRepository.GetAsync(tenantId, expenseId, cancellationToken);
        return expense is null
            ? Result<ExpenseModel>.Failure(ApplicationError.NotFound("Expense was not found."))
            : Result<ExpenseModel>.Success(expense);
    }

    public async Task<Result<IReadOnlyCollection<ExpenseModel>>> ListExpensesAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            return Result<IReadOnlyCollection<ExpenseModel>>.Failure(ApplicationError.Validation("TenantId must not be empty."));

        if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
            return Result<IReadOnlyCollection<ExpenseModel>>.Failure(ApplicationError.Validation("From date must not be after To date."));

        return Result<IReadOnlyCollection<ExpenseModel>>.Success(await expenseRepository.ListAsync(tenantId, from?.Date, to?.Date, cancellationToken));
    }

    private static ExpenseModel Map(Expense expense) =>
        new(expense.Id, expense.TenantId, expense.CategoryId, expense.ExpenseDate, expense.Amount, expense.Description, expense.BankTransactionId);
}
