# PW-AI-001 — Create AI provider abstraction

Status: implemented

## Goal
Define an Application-facing AI interpretation interface without coupling business logic to an AI SDK or vendor.

## Implementation
- `IAiProvider` exposes a cancellation-aware interpretation operation.
- `AiInterpretationRequest` carries an explicit context type, prompt and serialized approved context.
- `AiInterpretationResponse` separates summary, insights and caveats.
- No AI SDK, HTTP client or provider credential is referenced by the Application project.

## Acceptance
- Application depends only on `IAiProvider`.
- Provider implementations can remain in Infrastructure.
- Requests are explicit and testable.
