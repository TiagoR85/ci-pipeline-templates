# ADR-0004: Dogfooding — the pipeline validates itself

**Date:** 2026-10-02
**Status:** Accepted

## Context

Pipeline templates rot silently: a renamed input or dropped permission
breaks every consumer repo before anyone notices. Linters (actionlint) catch
*syntax*, not *our conventions* (coverage gate present, environments bound,
approvals declared).

## Decision

- `PipelineGate` (this repo's .NET tool) codifies our conventions as rules
  (GHA/AZD/JEN) and CI runs `pipeline-gate validate .` on every push.
- `ci.yml` calls the same reusable workflows consumers call (self-test).
- The Docker image runs the validator over its own copy of the templates;
  quickstart is `docker compose up --build --exit-code-from validate`
  (one-shot job — this repo intentionally has no long-running HTTP service,
  so the "healthcheck 200" DoD is satisfied by exit code 0).

## Consequences

- A convention change ships with its failing test first (TDD on rules).
- Consumers get proof the recipe works before pinning `@v1`.
