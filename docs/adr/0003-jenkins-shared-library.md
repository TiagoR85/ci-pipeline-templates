# ADR-0003: Jenkins shared library instead of copy-pasted Jenkinsfiles

**Date:** 2026-10-02
**Status:** Accepted

## Context

Jenkins pipelines are per-repo Groovy files; copy-paste drifts across
repositories (different coverage gates, missing approvals).

## Decision

Ship a shared library: `vars/dotnetPipeline.groovy` exposes
`call(Map config)` with build + coverage-gate stages. Repositories keep a
thin `Jenkinsfile` that loads the library with `@Library(...) _`, adds the
PROD `input` approval step and deploy steps.

## Consequences

- Gate logic lives in one place; a gate change is a library version bump.
- The validator enforces both sides: `Jenkinsfile*` must declare declarative
  syntax, `@Library` and `input`; `vars/*.groovy` must expose `call(...)`.
