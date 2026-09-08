# Personal Wealth Platform

A production-oriented personal wealth management platform designed around deterministic financial data, modular architecture, auditable calculations, and optional on-demand AI analysis.

## Architecture foundation

The implementation follows a modular-monolith approach with Clean Architecture boundaries. The initial solution contains API, Application, Domain, Infrastructure, Contracts, Worker, Shared, and dedicated test projects.

Dependency direction:

```text
API -> Application -> Domain
Infrastructure -> Application / Domain
Modules -> Domain / Application according to defined boundaries
```

The Domain layer must remain independent of API, EF Core, SQL Server, filesystem, AI SDKs, and notification SDKs.

## Current milestone

Phase 0 — Product and Engineering Foundation.

Current story: `PW-CORE-001 Create solution structure`.

The financial-domain implementation starts only after the engineering foundation is established.
