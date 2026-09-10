# Phase 12 — Angular Application

## PW-UI-001 — Application shell
Standalone Angular workspace with responsive shell, navigation and router outlet. The web client is isolated under `web/PersonalWealth.Web`.

## PW-UI-002 — Typed API client
ApiService provides typed dashboard and investment contracts and uses Angular HttpClient. Financial calculations remain server-side.

## PW-UI-003 — Authentication/session boundary
The UI is designed as an API client boundary; authentication integration is kept separate from financial components so an OIDC/JWT provider can be introduced without changing domain screens.

## PW-UI-004 — Banking screens
Routed Banking screen establishes the accounts/transactions operational surface backed by versioned banking APIs.

## PW-UI-005 — Expense screens
Routed Expenses screen establishes the expense operational surface and preserves the Application layer as the source of reporting rules.

## PW-UI-006 — Investment/asset/liability screens
Routed screens expose investment holdings and navigation surfaces for assets and liabilities. Data is read through typed API contracts rather than recalculated in the browser.

## PW-UI-007 — Wealth dashboard
Dashboard presents cash, investments, other assets, liabilities and net worth from the API.

## PW-UI-008 — Import/admin screens
Imports screen establishes the deterministic import-validation entry point. Administrative controls remain bounded and do not bypass API authorization.
