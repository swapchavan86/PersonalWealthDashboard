using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Infrastructure.Persistence;

internal static class TenantPersistenceEnforcement
{
    public static void ApplyQueryFilters(ModelBuilder modelBuilder, PersonalWealthDbContext dbContext)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(entityType => typeof(ITenantOwned).IsAssignableFrom(entityType.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(BuildQueryFilter(entityType.ClrType, dbContext));

            modelBuilder.Entity(entityType.ClrType)
                .HasIndex(nameof(ITenantOwned.TenantId));
        }
    }

    public static void Validate(ChangeTracker changeTracker, ITenantContext tenantContext)
    {
        foreach (var entry in changeTracker.Entries<ITenantOwned>())
        {
            if (entry.Entity.TenantId == Guid.Empty || entry.Entity.TenantId != tenantContext.TenantId)
            {
                throw new InvalidOperationException(
                    "Tenant-owned entities must use the active tenant context.");
            }
        }
    }

    private static LambdaExpression BuildQueryFilter(Type entityType, PersonalWealthDbContext dbContext)
    {
        var parameter = Expression.Parameter(entityType, "entity");
        var tenantId = Expression.Property(
            Expression.Convert(parameter, typeof(ITenantOwned)),
            nameof(ITenantOwned.TenantId));
        var activeTenantId = Expression.Property(
            Expression.Constant(dbContext),
            nameof(PersonalWealthDbContext.CurrentTenantId));

        return Expression.Lambda(Expression.Equal(tenantId, activeTenantId), parameter);
    }
}
