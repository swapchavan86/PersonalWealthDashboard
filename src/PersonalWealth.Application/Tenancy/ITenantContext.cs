namespace PersonalWealth.Application.Tenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
}
