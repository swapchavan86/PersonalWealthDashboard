# PW-CORE-002 — Enforce Clean Architecture dependency rules

## Goal
Protect the solution with automated architecture tests.

## Acceptance Criteria
- Domain has no dependency on API, Infrastructure, EF Core, SQL Server, filesystem, AI SDKs or notification SDKs.
- Application has no dependency on API or Infrastructure.
- Infrastructure may depend inward on Application/Domain.
- Architecture tests include valid and forbidden dependency cases.
- Tests fail when forbidden dependencies are introduced.
- No business functionality is implemented.

## Constraints
Follow Clean Architecture, SOLID, dependency inversion and interface segregation strongly. Keep comments minimal and English-only. Do not add secrets or hard-coded credentials.

## Validation
`dotnet restore`, `dotnet build`, `dotnet test`.

## Status
Ready
