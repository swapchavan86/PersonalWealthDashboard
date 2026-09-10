# Phase 11 — Identity and Tenancy

## PW-AUTH-001 — User identity boundary
UserIdentity is tenant-owned and stores external identity subject, email and active state. Domain does not know an identity-provider SDK.

## PW-AUTH-002 — Tenant lifecycle
Tenant owns name and active state. Deactivation is explicit and tenant IDs are never inferred from arbitrary user input in domain logic.

## PW-AUTH-003 — Tenant resolution
HttpTenantContext resolves tenant_id from the authenticated claim and supports the explicit tenant header boundary. Missing/invalid context fails rather than defaulting to a tenant.

## PW-AUTH-004 — Authorization
Controllers use ASP.NET authorization while persistence independently enforces TenantId ownership. This gives defense in depth between transport and storage.

## PW-AUTH-005 — Secure configuration
Authentication authority/audience and database/storage settings are configuration-driven. No production secret is committed.

## PW-AUTH-006 — Integration tests
Persistence tests prove tenant-owned models can be stored through a tenant context. Full identity-provider integration remains environment-specific and must be exercised in CI/deployment configuration.
