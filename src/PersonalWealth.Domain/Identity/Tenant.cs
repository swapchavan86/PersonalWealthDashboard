using PersonalWealth.Domain.Entities;
namespace PersonalWealth.Domain.Identity;
public sealed class Tenant : Entity<Guid>, IAuditableEntity
{
    private Tenant() : base(Guid.NewGuid()) { }
    public Tenant(Guid id, string name) : base(id) { if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.",nameof(name)); Name=name.Trim(); IsActive=true; }
    public string Name { get; private set; }=null!; public bool IsActive { get; private set; } public DateTime CreatedAt { get; private set; } public DateTime? UpdatedAt { get; private set; }
    public void Deactivate()=>IsActive=false;
}
public sealed class UserIdentity : TenantEntity<Guid>, IAuditableEntity
{
    private UserIdentity() : base(Guid.NewGuid(),Guid.NewGuid()) { }
    public UserIdentity(Guid id, Guid tenantId, string subject, string email) : base(id,tenantId) { if(string.IsNullOrWhiteSpace(subject))throw new ArgumentException("Subject is required.",nameof(subject)); if(string.IsNullOrWhiteSpace(email))throw new ArgumentException("Email is required.",nameof(email)); Subject=subject.Trim(); Email=email.Trim().ToLowerInvariant(); IsActive=true; }
    public string Subject {get;private set;}=null!; public string Email {get;private set;}=null!; public bool IsActive {get;private set;} public DateTime CreatedAt {get;private set;} public DateTime? UpdatedAt {get;private set;}
    public void Deactivate()=>IsActive=false;
}
