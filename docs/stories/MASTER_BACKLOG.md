# Personal Wealth Platform — Master Story Backlog

This backlog is the implementation contract for Codex. Stories are executed in order unless an ADR explicitly changes the dependency. A feature branch/PR may contain a logical batch of sequential stories when the batch produces a complete, testable checkpoint.

## Phase 0 — Engineering Foundation
- PW-CORE-001 Create solution structure [implemented]
- PW-CORE-002 Enforce Clean Architecture dependency rules [implemented]
- PW-CORE-003 Create base entity contracts [implemented]
- PW-CORE-004 Create tenant-aware entity contract [implemented]
- PW-CORE-005 Create domain event contract [implemented]
- PW-CORE-006 Create application result/error model [implemented]
- PW-CORE-007 Harden architecture test suite [implemented]

## Phase 1 — Database Foundation
- PW-DB-001 Configure SQL Server and environment configuration [implemented]
- PW-DB-002 Configure EF Core Code First conventions [implemented]
- PW-DB-003 Create initial database migration [implemented]
- PW-DB-004 Add audit and concurrency conventions [implemented]
- PW-DB-005 Add tenant isolation/query filtering foundation [implemented]
- PW-DB-006 Add database integration-test infrastructure [implemented]

## Phase 2 — Events and Reliability
- PW-EVENT-001 Create event bus abstraction [implemented]
- PW-EVENT-002 Implement in-process event bus [implemented]
- PW-EVENT-003 Register event handlers and dispatching rules [implemented]
- PW-EVENT-004 Implement transactional outbox [implemented]
- PW-EVENT-005 Implement outbox publisher worker [implemented]
- PW-EVENT-006 Implement idempotency and processed-event handling [implemented]

## Phase 3 — Documents and Deterministic Import
- PW-DOC-001 Configure master data/document folders [implemented]
- PW-DOC-002 Create document metadata and hashing [implemented]
- PW-DOC-003 Implement folder scanner and file discovery [implemented]
- PW-DOC-004 Define import template abstraction [implemented]
- PW-DOC-005 Define tabular template/schema model [implemented]
- PW-DOC-006 Implement schema and structural validation [implemented]
- PW-DOC-007 Implement business validation pipeline [implemented]
- PW-DOC-008 Implement staging and import lifecycle [implemented]
- PW-DOC-009 Implement duplicate detection and idempotent import [implemented]
- PW-DOC-010 Implement canonical import commit and import events [implemented]
- PW-DOC-011 Implement first bank-statement document template [implemented]
- PW-DOC-012 Implement bank document parsing/normalization adapter [implemented]

## Phase 4 — Banking
- PW-BANK-001 Create bank account domain model [implemented]
- PW-BANK-002 Create bank transaction domain model [implemented]
- PW-BANK-003 Implement banking application interfaces/use cases [implemented]
- PW-BANK-004 Implement bank transaction categorization and normalization [implemented]
- PW-BANK-005 Implement transfer detection and linking [implemented]
- PW-BANK-006 Implement reconciliation and statement balance rules [implemented]
- PW-BANK-007 Implement bank import business workflow [implemented]
- PW-BANK-008 Add banking integration tests and invariants [implemented]

## Phase 5 — Expenses
- PW-EXP-001 Create expense/category domain model [implemented]
- PW-EXP-002 Implement expense application use cases [implemented]
- PW-EXP-003 Implement recurring expense rules [implemented]
- PW-EXP-004 Implement expense aggregation/reporting queries [implemented]
- PW-EXP-005 Link bank transactions to expenses/categories [implemented]
- PW-EXP-006 Add expense tests [implemented]

## Phase 6 — Investments
- PW-INV-001 Create investment account/portfolio model [implemented]
- PW-INV-002 Create security/instrument master model [implemented]
- PW-INV-003 Create holdings and transaction model [implemented]
- PW-INV-004 Implement investment business rules [implemented]
- PW-INV-005 Implement market-price provider abstraction [implemented]
- PW-INV-006 Implement valuation and gain/loss calculations [implemented]
- PW-INV-007 Implement corporate-action handling foundation
- PW-INV-008 Implement investment import templates
- PW-INV-009 Implement investment reconciliation
- PW-INV-010 Add investment integration tests

## Phase 7 — Assets
- PW-ASSET-001 Create physical/financial asset model
- PW-ASSET-002 Implement asset valuation and ownership rules
- PW-ASSET-003 Implement asset history/snapshots
- PW-ASSET-004 Implement asset import workflow
- PW-ASSET-005 Add asset tests

## Phase 8 — Liabilities
- PW-LIAB-001 Create liability/loan model
- PW-LIAB-002 Create repayment and schedule model
- PW-LIAB-003 Implement interest/principal calculations
- PW-LIAB-004 Implement credit-card liability workflow
- PW-LIAB-005 Implement EPF/NPS liability/retirement-account modeling where applicable
- PW-LIAB-006 Implement liability snapshots and ratios
- PW-LIAB-007 Add liability tests

## Phase 9 — Wealth Engine
- PW-WEALTH-001 Create wealth calculation contracts
- PW-WEALTH-002 Calculate total assets and liabilities
- PW-WEALTH-003 Calculate net worth and growth
- PW-WEALTH-004 Calculate allocation and concentration
- PW-WEALTH-005 Calculate cash flow, savings rate and expense ratios
- PW-WEALTH-006 Calculate debt and liquidity ratios
- PW-WEALTH-007 Implement historical wealth snapshots
- PW-WEALTH-008 Add deterministic wealth-engine test suite

## Phase 10 — API Foundation
- PW-API-001 Configure API conventions and versioning
- PW-API-002 Implement validation/error handling pipeline
- PW-API-003 Implement authentication/authorization boundary
- PW-API-004 Implement banking APIs
- PW-API-005 Implement wealth/dashboard APIs
- PW-API-006 Implement import/document APIs

## Phase 11 — Identity and Tenancy
- PW-AUTH-001 Create user/identity model boundary
- PW-AUTH-002 Create tenant model and lifecycle
- PW-AUTH-003 Implement tenant resolution
- PW-AUTH-004 Implement authorization policies
- PW-AUTH-005 Implement secure secret/configuration integration
- PW-AUTH-006 Add identity/tenant integration tests

## Phase 12 — Angular Application
- PW-UI-001 Create Angular application shell
- PW-UI-002 Create API client and typed contracts
- PW-UI-003 Create authentication/session boundary
- PW-UI-004 Create banking screens
- PW-UI-005 Create expense screens
- PW-UI-006 Create investment/asset/liability screens
- PW-UI-007 Create wealth dashboard
- PW-UI-008 Create import/admin screens

## Phase 13 — Alerts and Notifications
- PW-ALERT-001 Create alert domain model
- PW-ALERT-002 Implement alert rules engine
- PW-ALERT-003 Implement notification provider abstraction
- PW-ALERT-004 Implement notification delivery workflow
- PW-ALERT-005 Add alert/notification tests

## Phase 14 — AI Interpretation
- PW-AI-001 Create AI provider abstraction
- PW-AI-002 Create deterministic financial context builders
- PW-AI-003 Implement stock/investment analysis use case
- PW-AI-004 Implement expense analysis use case
- PW-AI-005 Implement credit-card analysis use case
- PW-AI-006 Implement EPF/NPS analysis use case
- PW-AI-007 Implement overall wealth analysis use case
- PW-AI-008 Add AI safety, privacy, audit and failure-path tests

## Phase 15 — Admin and Operations
- PW-ADMIN-001 Create configuration/admin boundaries
- PW-ADMIN-002 Create import/job monitoring
- PW-ADMIN-003 Create audit/diagnostic views
- PW-ADMIN-004 Add operational controls and health checks

## Phase 16 — Deployment and Production Hardening
- PW-OPS-001 Configure local IIS deployment
- PW-OPS-002 Configure Docker/container deployment
- PW-OPS-003 Configure CI build/test pipeline
- PW-OPS-004 Configure secure production configuration/secrets
- PW-OPS-005 Configure structured logging and correlation
- PW-OPS-006 Configure metrics/tracing/health endpoints
- PW-OPS-007 Perform security and tenant-isolation hardening
- PW-OPS-008 Perform production readiness validation

## Global acceptance rules
Every story must preserve Clean Architecture, SOLID, tenant isolation, deterministic financial calculations, secure configuration, limited English-only comments, testability, and production quality. No story may silently implement future business features. EF Core is Code First and migrations are source-controlled. Local secrets are local-only; production secrets are supplied by secure environment/secret-management configuration.
