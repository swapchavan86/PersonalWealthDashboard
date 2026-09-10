using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Banking;

public sealed class ImportIdentity : TenantEntity<Guid>, IAuditableEntity
{
    private ImportIdentity()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public ImportIdentity(Guid id, Guid tenantId, Guid importId, string contentHash)
        : base(id, tenantId)
    {
        if (importId == Guid.Empty) throw new ArgumentException("ImportId must not be empty.", nameof(importId));
        if (string.IsNullOrWhiteSpace(contentHash)) throw new ArgumentException("Content hash is required.", nameof(contentHash));
        ImportId = importId;
        ContentHash = contentHash.Trim().ToLowerInvariant();
    }

    public Guid ImportId { get; private set; }
    public string ContentHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}

public sealed class ImportDuplicateDecision : TenantEntity<Guid>, IAuditableEntity
{
    private ImportDuplicateDecision()
        : base(Guid.NewGuid(), Guid.NewGuid())
    {
    }

    public ImportDuplicateDecision(Guid id, Guid tenantId, Guid importId, string scope, string fingerprint, bool isDuplicate, string reason)
        : base(id, tenantId)
    {
        ImportId = importId;
        Scope = scope.Trim();
        Fingerprint = fingerprint.Trim();
        IsDuplicate = isDuplicate;
        Reason = reason.Trim();
    }

    public Guid ImportId { get; private set; }
    public string Scope { get; private set; } = null!;
    public string Fingerprint { get; private set; } = null!;
    public bool IsDuplicate { get; private set; }
    public string Reason { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
