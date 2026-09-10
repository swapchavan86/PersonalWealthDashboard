namespace PersonalWealth.Domain.Entities;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}

public interface IConcurrencyTracked
{
    byte[] RowVersion { get; }
}
