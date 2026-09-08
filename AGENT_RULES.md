# Codex Engineering Rules

## Architecture
- Build the platform as a modular monolith initially.
- Follow Clean Architecture and pragmatic DDD.
- Dependency direction is inward: API -> Application -> Domain; Infrastructure -> Application/Domain.
- Domain must not depend on API, ASP.NET Core, EF Core, SQL Server, filesystem, external SDKs, AI SDKs, or notification SDKs.
- Application owns use-case orchestration and depends on abstractions, not infrastructure implementations.
- Interfaces required by use cases belong in Application unless they are genuine domain abstractions.
- Follow SOLID principles strongly, especially single responsibility, dependency inversion, interface segregation, and open/closed design.
- Do not introduce microservices until extraction is justified by a documented architectural decision.
- Keep business rules deterministic, testable, auditable, and independent of infrastructure.

## Database
- Use EF Core Code First.
- Database design must be derived from domain/business requirements, not from UI screens.
- Migrations are source-controlled artifacts.
- Do not use database-first scaffolding as the primary design approach.
- TenantId must be designed into tenant-owned data from the beginning.
- Audit fields and concurrency/idempotency requirements must be considered for persistent entities.

## Configuration and Secrets
- Never hard-code secrets, credentials, tokens, API keys, certificates, or private keys.
- Local development may use local-only configuration such as appsettings.Local.json, user secrets, or environment variables.
- Local secret files must be ignored by Git and must never be committed.
- Production configuration must come from secure environment configuration, environment variables, or a managed secret store.
- Application/business code must consume configuration through abstractions/options rather than assuming a specific production secret store.
- Provide safe sample configuration with placeholders only.

## Comments
- Keep code comments minimal.
- All code comments must be in English.
- Do not comment obvious code.
- Use comments only for genuinely non-obvious architecture, security, complex algorithms, or business rules.
- Prefer expressive names and structure over explanatory comments.

## Story Workflow
- Treat docs/stories as the implementation contract.
- Implement exactly one story per feature branch and PR unless the story explicitly defines a coordinated set of changes.
- Do not implement future stories opportunistically.
- Before coding, read README.md, AGENT_RULES.md, architecture documentation, ADRs, the target story, and relevant existing code.
- Inspect existing code before introducing new abstractions or packages.
- Add or update tests for every behavior and architectural rule introduced.
- Run restore, build, and test before opening a PR.
- Review the complete diff for unrelated changes, security issues, tenant isolation, architecture violations, and unnecessary comments.
- Update the story status and validation section according to repository conventions.
- Open a PR targeting main.
- Do not merge PRs automatically.

## Financial Data
- User owns data entry.
- Application owns calculations.
- AI owns interpretation only.
- AI must never be required for deterministic financial calculations or normal data synchronization.
- Preserve raw imported data where appropriate for auditability and troubleshooting.
- Imports must be idempotent and duplicate-safe.

## Bank Document / Template Rule
- Bank statement ingestion starts from a deterministic document/template pipeline.
- The first bank document format must have an explicit template/schema definition before the business import flow is considered complete.
- Document parsing, normalization, validation, staging, duplicate detection, and canonical transaction creation are separate responsibilities.
- Business-layer rules must not depend directly on PDF/Excel/CSV libraries.

## Production Quality
- Avoid speculative abstractions and premature optimization.
- Do not bypass validation to make tests pass.
- Do not weaken architecture rules to accommodate implementation convenience.
- Do not introduce unrelated refactoring.
