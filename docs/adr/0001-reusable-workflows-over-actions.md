# ADR-0001: Reusable workflows over composite actions and marketplace actions

**Date:** 2026-10-02
**Status:** Accepted

## Context

The portfolio needs one CI recipe (build → tests → coverage gate) reused by
15 repositories, plus Docker scanning and deploy templates. Options:
composite actions, reusable workflows, or pulling actions from the
marketplace.

## Decision

- **GitHub Actions:** reusable workflows (`on: workflow_call`) living in
  `.github/workflows/`, called with `uses: <owner>/<repo>/.github/workflows/
  <file>.yml@<tag>` (or `./` for self-testing).
- **Azure DevOps:** stage/job/step templates under `azure-devops/templates/`.
- **Jenkins:** shared library with an entry point in `vars/`.

## Alternatives considered

- **Composite actions:** cannot define jobs/runners/secrets scoping — poor
  fit for a full pipeline.
- **Marketplace actions:** third-party trust + version drift; CI is the
  security perimeter, we keep it in-repo and versioned by git tag.

## Consequences

- Consumers pin `@v1` tags; breaking changes are SemVer-major tags.
- Self-test in `ci.yml` calls the same reusable workflow consumers use, so a
  broken recipe breaks *our* CI first (see ADR-0004).
