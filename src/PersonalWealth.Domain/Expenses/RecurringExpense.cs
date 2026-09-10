using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Expenses;

public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public sealed class RecurringExpense : TenantEntity<Guid>
{
    private RecurringExpense()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
        Description = string.Empty;
    }

    public RecurringExpense(
        Guid id,
        Guid tenantId,
        Guid categoryId,
        DateTime startDate,
        decimal amount,
        string description,
        RecurrenceFrequency frequency,
        int interval = 1,
        DateTime? endDate = null)
        : base(id, tenantId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId must not be empty.", nameof(categoryId));
        if (startDate == default) throw new ArgumentException("Start date is required.", nameof(startDate));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (interval <= 0) throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be greater than zero.");
        if (endDate.HasValue && endDate.Value.Date < startDate.Date) throw new ArgumentException("End date must not be before start date.", nameof(endDate));

        CategoryId = categoryId;
        StartDate = startDate.Date;
        Amount = amount;
        Description = description.Trim();
        Frequency = frequency;
        Interval = interval;
        EndDate = endDate?.Date;
        IsActive = true;
    }

    public Guid CategoryId { get; }
    public DateTime StartDate { get; }
    public decimal Amount { get; }
    public string Description { get; }
    public RecurrenceFrequency Frequency { get; }
    public int Interval { get; }
    public DateTime? EndDate { get; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<DateTime> GetOccurrences(DateTime from, DateTime to)
    {
        if (from.Date > to.Date) throw new ArgumentException("From date must not be after To date.");

        var effectiveFrom = from.Date < StartDate ? StartDate : from.Date;
        var effectiveTo = EndDate.HasValue && EndDate.Value < to.Date ? EndDate.Value : to.Date;
        if (!IsActive || effectiveFrom > effectiveTo) return Array.Empty<DateTime>();

        var occurrences = new List<DateTime>();
        var current = StartDate;
        while (current < effectiveFrom) current = Next(current);

        while (current <= effectiveTo)
        {
            occurrences.Add(current);
            current = Next(current);
        }

        return occurrences;
    }

    public void Deactivate() => IsActive = false;

    private DateTime Next(DateTime date) => Frequency switch
    {
        RecurrenceFrequency.Daily => date.AddDays(Interval),
        RecurrenceFrequency.Weekly => date.AddDays(7 * Interval),
        RecurrenceFrequency.Monthly => date.AddMonths(Interval),
        RecurrenceFrequency.Yearly => date.AddYears(Interval),
        _ => throw new ArgumentOutOfRangeException()
    };
}
