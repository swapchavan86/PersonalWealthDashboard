using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PersonalWealth.Application.Tenancy;
namespace PersonalWealth.Infrastructure.Tenancy;
public sealed class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    public Guid TenantId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue("tenant_id") ?? accessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            return Guid.TryParse(value, out var id) && id != Guid.Empty ? id : throw new InvalidOperationException("A valid tenant context is required.");
        }
    }
}
