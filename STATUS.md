# Status do Projeto — v1.0.0 (2026-11-12)

## Estado Atual

Biblioteca de templates reutilizáveis de CI/CD para .NET em três
plataformas — GitHub Actions (workflows reutilizáveis), Azure DevOps
(templates de stage com DEV/HOM/PROD + aprovações) e Jenkins (shared
library) — consumida pelos repositórios do portfólio em vez de
copiar/colar YAML. Inclui um validador próprio (`PipelineGate`, .NET 10)
que valida cada template, roda no CI deste repo (dogfooding, ADR-0004) e
é exposto como reusable workflow `reusable-doccheck.yml`, já adotado por
outros 5 repos do portfólio. Gate de cobertura por módulo (default 80%)
e CodeQL fazem parte do pacote.

## Fases

| Fase | Estado | Observação |
|---|---|---|
| Reusable workflows GitHub Actions (.NET + Node/Astro) | ✅ Concluído | `.github/workflows/reusable-dotnet-ci.yml`, `reusable-node-ci.yml` |
| Templates Azure DevOps (build/test-gate/deploy-stage) | ✅ Concluído | `azure-devops/templates/` + sample |
| Jenkins shared library | ✅ Concluído | `jenkins/vars/dotnetPipeline.groovy` |
| Validador PipelineGate com testes | ✅ Concluído | `src/PipelineGate` + `PipelineGate.Tests` (xunit) |
| Dogfooding (compose valida os próprios templates) | ✅ Concluído | ADR-0004, `docker compose up --exit-code-from validate` |
| Gate de docs (reusable-doccheck) | ✅ Concluído | commits `chore(doccheck)` 0.3–0.5, adotado por 5 repos |
| Publicação formal de versões (tagging das workflows) | ⏳ Pendente | consumers fixam `@v1`, mas tag física é só `v1` |

## Pendências

- [ ] Tagging semver formal das workflows (`v1.0.x`, não só `v1`)
- [ ] Ampliar cobertura de validação do PipelineGate conforme convenções novas
- [ ] Documentar em README a lista dos repos que adotam cada workflow (commit `docs(ci)` existe)
