using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Expenses;

public sealed class ExpenseCategory : TenantEntity<Guid>
{
    private ExpenseCategory()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public ExpenseCategory(Guid id, Guid tenantId, string name, Guid? parentCategoryId = null)
        : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        if (parentCategoryId == id)
            throw new ArgumentException("A category cannot be its own parent.", nameof(parentCategoryId));

        Name = name.Trim();
        ParentCategoryId = parentCategoryId;
        IsActive = true;
    }

    public string Name { get; private set; } = null!;
    public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        Name = name.Trim();
    }

    public void SetParent(Guid? parentCategoryId)
    {
        if (parentCategoryId == Id)
            throw new ArgumentException("A category cannot be its own parent.", nameof(parentCategoryId));

        ParentCategoryId = parentCategoryId;
    }

    public void Deactivate() => IsActive = false;
}
