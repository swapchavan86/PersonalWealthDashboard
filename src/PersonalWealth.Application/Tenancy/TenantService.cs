using PersonalWealth.Domain.Identity;
namespace PersonalWealth.Application.Tenancy;
public sealed record TenantDescriptor(Guid Id,string Name,bool IsActive);
public interface ITenantService { TenantDescriptor Describe(Tenant tenant); bool CanAccess(UserIdentity user, Guid tenantId); }
public sealed class TenantService : ITenantService
{
    public TenantDescriptor Describe(Tenant tenant)=>new(tenant.Id,tenant.Name,tenant.IsActive);
    public bool CanAccess(UserIdentity user,Guid tenantId)=>user.IsActive && user.TenantId==tenantId;
}
