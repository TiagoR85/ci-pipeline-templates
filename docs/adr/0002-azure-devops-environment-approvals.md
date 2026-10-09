# ADR-0002: Approvals via Azure DevOps Environments, not YAML

**Date:** 2026-10-02
**Status:** Accepted

## Context

The spec requires DEV/HOM/PROD stages with approval before production.
Azure DevOps approval checks cannot be declared in pipeline YAML.

## Decision

Each deploy template binds the deployment job to an ADO **Environment**
(`environment: ${{ parameters.environment }}`). Manual validation checks are
configured once per environment in the ADO UI (Environments → DEV/HOM/PROD →
Approvals and checks). The pipeline only references the environment name.

## Consequences

- YAML stays environment-agnostic; approval policy lives with the
  environment, not with the pipeline (same principle as GitHub Environments).
- First-time users must create the three environments and attach the
  approvals — documented in the README quickstart.
