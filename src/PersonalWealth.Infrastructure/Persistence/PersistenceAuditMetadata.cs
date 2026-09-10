using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Infrastructure.Persistence;

public static class PersistenceAuditMetadata
{
    public static void Apply(ChangeTracker changeTracker, DateTime utcNow)
    {
        foreach (var entry in changeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = null;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                    break;
            }
        }
    }
}
